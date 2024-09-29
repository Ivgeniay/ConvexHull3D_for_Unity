using System.Collections.Generic;
using System.Linq;

namespace MvConvex
{
    /// <summary>
    /// Основной класс для вычисления выпуклой оболочки в трехмерном пространстве.
    /// </summary>
    /// <typeparam name="TVertex">Тип вершины.</typeparam>
    /// <typeparam name="TTriangle">Тип треугольника.</typeparam>
    /// <typeparam name="TFace">Тип грани.</typeparam>
    /// <typeparam name="TEdge">Тип ребра.</typeparam>
    /// <typeparam name="TResult">Тип результата.</typeparam>
    public class ConvexHull3D<TVertex, TTriangle, TFace, TEdge, TResult>
        where TVertex : class, IVertex
        where TTriangle : class, ITriangle, new()
        where TFace : class, IFace<TVertex, TTriangle>, new()
        where TEdge : class, IEdge, new()
        where TResult : class, IConvexHullResult<TVertex, TTriangle, TFace, TEdge>, new()
    {
        private readonly List<TVertex> allVertices;
        private readonly TetrahedronBuilder<TVertex, TTriangle, TFace> tetrahedronBuilder;
        private readonly HullExpander<TVertex, TTriangle, TFace> hullExpander;
        private readonly FaceManager<TVertex, TTriangle, TFace> faceManager;
        private readonly ILogger logger; 

        public ConvexHull3D(IEnumerable<TVertex> vertices, ILogger logger = null)
        {
            this.allVertices = vertices.ToList();
            this.logger = logger;

            if (this.allVertices.Count < 4)
            {
                logger?.LogError("Insufficient points provided to construct a 3D convex hull.");
                throw new InsufficientPointsException();
            }

            tetrahedronBuilder = new TetrahedronBuilder<TVertex, TTriangle, TFace>(this.allVertices, logger);
            hullExpander = new HullExpander<TVertex, TTriangle, TFace>(logger);
            faceManager = new FaceManager<TVertex, TTriangle, TFace>(logger);
             
            logger?.LogInformation($"ConvexHull3D initialized with {this.allVertices.Count} vertices.");
        }

        public TResult CalculateConvexHull()
        {
            logger?.LogInformation("Starting convex hull calculation.");

            List<TFace> faces;
            try
            {
                faces = tetrahedronBuilder.CreateInitialTetrahedron();
            }
            catch (DegenerateTetrahedronException ex)
            {
                logger?.LogError(ex, "Failed to create initial tetrahedron.");
                throw;
            }

            var remainingVertices = new List<TVertex>(allVertices);
            remainingVertices.RemoveAll(v => faces.Any(f => f.Triangles.Any(t => t.Contains(v))));

            logger?.LogInformation($"Initial tetrahedron created. Remaining vertices: {remainingVertices.Count}");

            int iterationsWithoutProgress = 0;
            int maxIterationsWithoutProgress = 10;

            while (remainingVertices.Count > 0 && iterationsWithoutProgress < maxIterationsWithoutProgress)
            {
                TVertex furthestVertex = FindFurthestVertex(remainingVertices, faces);
                if (furthestVertex == null) break;

                int oldFaceCount = faces.Count;
                faces = hullExpander.ExpandConvexHull(furthestVertex, faces);

                if (faces.Count == oldFaceCount)
                {
                    iterationsWithoutProgress++;
                    logger?.LogWarning($"No progress made in iteration. Iterations without progress: {iterationsWithoutProgress}");
                }
                else
                {
                    iterationsWithoutProgress = 0;
                    logger?.LogInformation($"Hull expanded. New face count: {faces.Count}");
                }

                remainingVertices.Remove(furthestVertex);
            }

            if (iterationsWithoutProgress >= maxIterationsWithoutProgress)
            {
                logger?.LogError($"Algorithm failed to make progress after {maxIterationsWithoutProgress} iterations.");
                throw new NoProgressException();
            }

            faceManager.UpdateFaceAdjacency(faces);

            logger?.LogInformation($"Convex hull calculation completed. Face count: {faces.Count}");

            return CreateResult(faces);
        }

        private TVertex FindFurthestVertex(List<TVertex> vertices, List<TFace> faces)
        {
            if (faces.Count == 0 || vertices.Count == 0)
            {
                logger?.LogWarning("FindFurthestVertex called with empty faces or vertices.");
                return null;
            }

            return vertices.ArgMax(v =>
            {
                double maxDistance = faces.SelectMany(f => f.Triangles)
                                          .Max(t => GeometryHelper.PointTriangleDistance(v.Position, t));
                return maxDistance > GeometryHelper.Epsilon ? maxDistance : 0;
            });
        }

        /// <summary>
        /// Создает результат построения выпуклой оболочки.
        /// </summary>
        private TResult CreateResult(List<TFace> faces)
        {
            var result = new TResult();
            var hullVertices = faces.SelectMany(f => f.Triangles)
                .SelectMany(t => new[] { t.VertexA, t.VertexB, t.VertexC })
                .Distinct()
                .Cast<TVertex>()
                .ToList();
            var hullTriangles = faces.SelectMany(f => f.Triangles)
                .Cast<TTriangle>()
                .ToList();

            var hullEdges = new HashSet<TEdge>();
            foreach (var triangle in hullTriangles)
            {
                hullEdges.Add(CreateEdge(triangle.VertexA, triangle.VertexB));
                hullEdges.Add(CreateEdge(triangle.VertexB, triangle.VertexC));
                hullEdges.Add(CreateEdge(triangle.VertexC, triangle.VertexA));
            }

            var interiorVertices = new List<TVertex>();
            var ignoredVertices = new List<TVertex>();

            foreach (var vertex in allVertices)
            {
                if (!hullVertices.Contains(vertex))
                {
                    if (IsInsideHull(vertex, faces))
                    {
                        interiorVertices.Add(vertex);
                    }
                    else if (IsCloseToAnyVertex(vertex, hullVertices))
                    {
                        ignoredVertices.Add(vertex);
                    }
                }
            }

            result.SetConvexHullData(hullVertices, hullTriangles, faces, hullEdges.ToList(), interiorVertices, ignoredVertices);

            logger?.LogInformation($"Result created. Hull vertices: {result.HullVertices.Count}, Hull edges: {result.HullEdges.Count}, Hull triangles: {result.HullTriangles.Count}, Hull faces: {result.HullFaces.Count}, Interior vertices: {result.InteriorVertices.Count}, Ignored vertices: {result.IgnoredVertices.Count}");
             
            return result;
        } 

        /// <summary>
        /// Создает ребро из двух вершин.
        /// </summary>
        private TEdge CreateEdge(IVertex a, IVertex b)
        {
            // Предполагается, что у вас есть конструктор для TEdge, принимающий две вершины
            TEdge edge = new TEdge();
            edge.Initialize(a, b);
            return edge;
        }


        /// <summary>
        /// Проверяет, находится ли вершина внутри выпуклой оболочки.
        /// </summary>
        private bool IsInsideHull(TVertex vertex, List<TFace> faces)
        {
            foreach (var face in faces)
            {
                foreach (var triangle in face.Triangles)
                {
                    if (triangle.IsPointInFront(vertex.Position))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsCloseToAnyVertex(TVertex vertex, List<TVertex> hullVertices)
        {
            return hullVertices.Any(v => (v.Position - vertex.Position).sqrMagnitude < GeometryHelper.Epsilon);
        }
    } 
}
