using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MvConvex
{ 
    public class HullExpander<TVertex, TTriangle, TFace>
        where TVertex : class, IVertex
        where TTriangle : class, ITriangle, new()
        where TFace : class, IFace<TVertex, TTriangle>, new()
    {

        private readonly ILogger logger; 
        public HullExpander(ILogger logger = null)
        {
            this.logger = logger;
        }

        public List<TFace> ExpandConvexHull(TVertex vertex, List<TFace> faces)
        {
            logger?.LogInformation($"Expanding convex hull with vertex at position {vertex.Position}");

            var visibleFaces = faces.Where(f => IsVisibleFace(vertex, f)).ToList();
            var horizon = FindHorizon(visibleFaces, faces);

            logger?.LogDebug($"Visible faces: {visibleFaces.Count}, Horizon edges: {horizon.Count}"); 

            foreach (var face in visibleFaces)
            {
                faces.Remove(face);
            }

            foreach (var edge in horizon)
            {
                faces.Add(CreateFace(vertex, edge.Start, edge.End));
            }

            logger?.LogInformation($"Convex hull expanded. New face count: {faces.Count}");

            return faces;
        }

        private bool IsVisibleFace(TVertex vertex, TFace face)
        {
            var triangle = face.Triangles[0];
            Vector3 normal = Vector3.Cross(
                triangle.VertexB.Position - triangle.VertexA.Position,
                triangle.VertexC.Position - triangle.VertexA.Position).normalized;
            return Vector3.Dot(vertex.Position - triangle.VertexA.Position, normal) > 1e-6f;
        }

        private List<IEdge> FindHorizon(List<TFace> visibleFaces, List<TFace> allFaces)
        {
            var horizon = new List<IEdge>();
            foreach (var face in visibleFaces)
            {
                foreach (var triangle in face.Triangles)
                {
                    CheckEdge(triangle.EdgeAB, visibleFaces, allFaces, horizon);
                    CheckEdge(triangle.EdgeBC, visibleFaces, allFaces, horizon);
                    CheckEdge(triangle.EdgeCA, visibleFaces, allFaces, horizon);
                }
            }
            return horizon;
        }

        private void CheckEdge(IEdge edge, List<TFace> visibleFaces, List<TFace> allFaces, List<IEdge> horizon)
        {
            var adjacentFace = allFaces.FirstOrDefault(f => !visibleFaces.Contains(f) && f.Triangles.Any(t => t.HasEdge(edge)));
            if (adjacentFace != null)
            {
                horizon.Add(edge);
            }
        }

        private TFace CreateFace(IVertex a, IVertex b, IVertex c)
        {
            var triangle = new TTriangle();
            triangle.Initialize(a, b, c);
            var face = new TFace();
            face.Initialize(new[] { triangle });
            return face;
        }
    }
}
