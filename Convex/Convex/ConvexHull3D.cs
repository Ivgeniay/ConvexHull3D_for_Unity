using System.Collections.Generic;
using System.Collections;
using System.Linq; 
using UnityEngine;
using System;

namespace MvConvex
{
    public class ConvexHull3D<TVertex, TTriangle, TFace, TResult>
        where TVertex : class, IVertex
        where TTriangle : class, ITriangle, new()
        where TFace : class, IFace<TVertex, TTriangle>, new()
        where TResult : class, IConvexHullResult<TVertex, TTriangle, TFace>, new()
    {
        private readonly IReadOnlyList<TVertex> vertices;
        private List<TTriangle> hull = new();
        private List<TVertex> remainingVertices = new(); 
        private List<TFace> faces = new();

        public ConvexHull3D(IEnumerable<TVertex> vertices)
        {
            this.vertices = vertices.ToList();
            if (this.vertices.Count < 4)
            {
                throw new ArgumentException("At least 4 non-coplanar points are required to construct a 3D convex hull.");
            }
        }

        /// <summary>
        /// Calculates the convex hull for the given set of vertices and returns the result.
        /// </summary>
        /// <returns>An object of type TResult representing the calculated convex hull.</returns>
        public TResult CalculateConvexHull()
        {
            hull = new List<TTriangle>();
            faces = new List<TFace>();
            remainingVertices = new List<TVertex>(vertices);

            CreateInitialTetrahedron();
            remainingVertices.RemoveAll(v => faces.Any(f => f.Triangles.Any(t => t.Contains(v))));

            while (remainingVertices.Count > 0)
            {
                TVertex furthestVertex = FindFurthestVertex();
                if (furthestVertex == null) break;

                ExpandConvexHull(furthestVertex);
            }

            UpdateFaceAdjacency();
            return CreateResult();
        }

        private TResult CreateResult()
        {
            var result = new TResult();
            result.SetConvexHullData(
                faces.SelectMany(f => f.Triangles).SelectMany(t => new[] { t.VertexA, t.VertexB, t.VertexC }).Distinct().Cast<TVertex>().ToList(),
                faces.SelectMany(f => f.Triangles).Cast<TTriangle>().ToList(),
                faces,
                vertices.Except(faces.SelectMany(f => f.Triangles).SelectMany(t => new[] { t.VertexA, t.VertexB, t.VertexC })).Cast<TVertex>().ToList()
            );
            return result;
        }

        private TVertex FindPointFurthestFromOrigin()
        {
            return vertices.OrderByDescending(v => v.Position.sqrMagnitude).First();
        }
                
        private TVertex FindPointFurthestFromLine(TVertex lineStart)
        {
            Vector3 lineDir = lineStart.Position.normalized;
            return vertices.OrderByDescending(v =>
                Vector3.Cross(v.Position - lineStart.Position, lineDir).sqrMagnitude).First();
        }
                
        private TVertex FindPointFurthestFromTriangle(TVertex a, TVertex b)
        {
            Vector3 normal = Vector3.Cross(b.Position - a.Position, Vector3.up).normalized;
            return vertices.OrderByDescending(v =>
                Mathf.Abs(Vector3.Dot(v.Position - a.Position, normal))).First();
        }
                
        private TVertex FindPointFurthestFromTriangle(TVertex a, TVertex b, TVertex c)
        {
            Vector3 normal = Vector3.Cross(b.Position - a.Position, c.Position - a.Position).normalized;
            return vertices.OrderByDescending(v =>
                Mathf.Abs(Vector3.Dot(v.Position - a.Position, normal))).First();
        }

        private void CreateInitialTetrahedron()
        {
            TVertex p0 = FindPointFurthestFromOrigin();
            TVertex p1 = FindPointFurthestFromLine(p0);
            TVertex p2 = FindPointFurthestFromTriangle(p0, p1);
            TVertex p3 = FindPointFurthestFromTriangle(p0, p1, p2);

            Vector3 v01 = p1.Position - p0.Position;
            Vector3 v02 = p2.Position - p0.Position;
            Vector3 v03 = p3.Position - p0.Position;
            if (Mathf.Abs(Vector3.Dot(v03, Vector3.Cross(v01, v02))) < float.Epsilon)
            {
                throw new InvalidOperationException("Unable to create a non-degenerate tetrahedron.");
            }

            AddFace(p0, p1, p2);
            AddFace(p0, p2, p3);
            AddFace(p0, p3, p1);
            AddFace(p1, p3, p2);
        }

        private void AddTriangle(IVertex a, IVertex b, IVertex c)
        {
            var triangle = new TTriangle();
            triangle.Initialize(a, b, c);
            hull.Add(triangle);
        }

        private TVertex FindFurthestVertex()
        {
            if (hull.Count == 0)
            {
                throw new InvalidOperationException("Hull is empty. Cannot find furthest vertex.");
            }
            return remainingVertices.ArgMax(v => hull.Max(t => PointTriangleDistance(v.Position, t)));
        }

        private void AddFace(IVertex a, IVertex b, IVertex c)
        {
            var triangle = new TTriangle();
            triangle.Initialize(a, b, c);
            hull.Add(triangle);
            var face = new TFace();
            face.Initialize(new[] { triangle });
            faces.Add(face);
        }

        private void ExpandConvexHull(TVertex vertex)
        {
            var visibleFaces = faces.Where(f => f.Triangles.Any(t => t.IsPointInFront(vertex.Position))).ToList();
            var horizon = FindHorizon(visibleFaces);

            foreach (var face in visibleFaces)
            {
                faces.Remove(face);
                hull.RemoveAll(t => face.Triangles.Contains(t));
            }

            foreach (var edge in horizon)
            {
                AddFace(vertex, edge.Start, edge.End);
            }

            remainingVertices.Remove(vertex);
        }

        private List<IEdge> FindHorizon(List<TFace> visibleFaces)
        {
            var horizon = new List<IEdge>();
            foreach (var face in visibleFaces)
            {
                foreach (var triangle in face.Triangles)
                {
                    CheckEdge(triangle.EdgeAB, visibleFaces, horizon);
                    CheckEdge(triangle.EdgeBC, visibleFaces, horizon);
                    CheckEdge(triangle.EdgeCA, visibleFaces, horizon);
                }
            }
            return horizon;
        }

        private void CheckEdge(IEdge edge, List<TFace> visibleFaces, List<IEdge> horizon)
        {
            var adjacentFace = faces.FirstOrDefault(f => !visibleFaces.Contains(f) && f.Triangles.Any(t => t.HasEdge(edge)));
            if (adjacentFace != null)
            {
                horizon.Add(edge);
            }
        }

        private void UpdateAdjacentTriangles()
        {
            foreach (var triangle in hull)
            {
                ITriangle adjacentAB = hull.FirstOrDefault(t => t != triangle && t.HasEdge(triangle.EdgeAB));
                ITriangle adjacentBC = hull.FirstOrDefault(t => t != triangle && t.HasEdge(triangle.EdgeBC));
                ITriangle adjacentCA = hull.FirstOrDefault(t => t != triangle && t.HasEdge(triangle.EdgeCA));

                triangle.SetAdjacentTriangles(adjacentAB, adjacentBC, adjacentCA);
            }
        }

        private static double PointTriangleDistance(Vector3 point, ITriangle triangle)
        {
            Vector3 normal = Vector3.Cross(
                triangle.VertexB.Position - triangle.VertexA.Position,
                triangle.VertexC.Position - triangle.VertexA.Position).normalized;

            return Math.Abs(Vector3.Dot(point - triangle.VertexA.Position, normal));
        }

        private void UpdateFaceAdjacency()
        {
            foreach (var face in faces)
            {
                face.Adjacency = faces.Where(f => f != face && SharesEdge(face, f)).ToArray();
            }
        }

        private bool SharesEdge(TFace face1, TFace face2)
        {
            return face1.Triangles.Any(t1 => face2.Triangles.Any(t2 =>
                t1.HasEdge(new Edge(t2.VertexA, t2.VertexB)) ||
                t1.HasEdge(new Edge(t2.VertexB, t2.VertexC)) ||
                t1.HasEdge(new Edge(t2.VertexC, t2.VertexA))));
        }
    } 
}
