using System.Collections.Generic; 
using UnityEngine;
using System; 

namespace MvConvex
{
    /// <summary>
    /// Класс для построения начального тетраэдра.
    /// </summary>
    public class TetrahedronBuilder<TVertex, TTriangle, TFace>
    where TVertex : class, IVertex
    where TTriangle : class, ITriangle, new()
    where TFace : class, IFace<TVertex, TTriangle>, new()
    {
        private readonly IReadOnlyList<TVertex> vertices;
        private readonly ILogger logger;

        public TetrahedronBuilder(IReadOnlyList<TVertex> vertices, ILogger logger = null)
        {
            this.vertices = vertices;
            this.logger = logger;
        }

        public List<TFace> CreateInitialTetrahedron()
        {
            logger?.LogInformation("Creating initial tetrahedron.");

            TVertex p0 = FindPointFurthestFromOrigin();
            TVertex p1 = FindPointFurthestFromPoint(p0);
            TVertex p2 = FindPointFurthestFromLine(p0, p1);
            TVertex p3 = FindPointFurthestFromTriangle(p0, p1, p2);

            Vector3 normal = Vector3.Cross(p1.Position - p0.Position, p2.Position - p0.Position).normalized;
            float volume = Mathf.Abs(Vector3.Dot(p3.Position - p0.Position, normal));

            if (volume < 1e-6f)
            {
                logger?.LogError("Failed to create a non-degenerate tetrahedron. Volume: {Volume}", volume);
                throw new DegenerateTetrahedronException();
            }

            var faces = new List<TFace>
                {
                    CreateFace(p0, p1, p2),
                    CreateFace(p0, p2, p3),
                    CreateFace(p0, p3, p1),
                    CreateFace(p1, p3, p2)
                };

            logger?.LogInformation($"Initial tetrahedron created with {faces.Count} faces.");

            return faces;
        }

        private TVertex FindPointFurthestFromOrigin()
        {
            return vertices.ArgMax(v => v.Position.sqrMagnitude);
        }

        private TVertex FindPointFurthestFromPoint(TVertex point)
        {
            return vertices.ArgMax(v => (v.Position - point.Position).sqrMagnitude);
        }

        private TVertex FindPointFurthestFromLine(TVertex a, TVertex b)
        {
            Vector3 lineDir = (b.Position - a.Position).normalized;
            return vertices.ArgMax(v => Vector3.Cross(v.Position - a.Position, lineDir).sqrMagnitude);
        }

        private TVertex FindPointFurthestFromTriangle(TVertex a, TVertex b, TVertex c)
        {
            Vector3 normal = Vector3.Cross(b.Position - a.Position, c.Position - a.Position).normalized;
            return vertices.ArgMax(v => Mathf.Abs(Vector3.Dot(v.Position - a.Position, normal)));
        }

        private TFace CreateFace(TVertex a, TVertex b, TVertex c)
        {
            var triangle = new TTriangle();
            triangle.Initialize(a, b, c);
            var face = new TFace();
            face.Initialize(new[] { triangle });
            return face;
        }
    }
}
