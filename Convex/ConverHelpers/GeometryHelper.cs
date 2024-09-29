using UnityEngine;
using System;

namespace MvConvex
{
    public static class GeometryHelper
    {
        public const float Epsilon = 1e-6f;

        public static double PointTriangleDistance(Vector3 point, ITriangle triangle)
        {
            Vector3 normal = Vector3.Cross(
                triangle.VertexB.Position - triangle.VertexA.Position,
                triangle.VertexC.Position - triangle.VertexA.Position).normalized;

            return Math.Abs(Vector3.Dot(point - triangle.VertexA.Position, normal));
        }

        public static bool ArePointsCoplanar(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            Vector3 normal = Vector3.Cross(b - a, c - a).normalized;
            return Mathf.Abs(Vector3.Dot(d - a, normal)) < Epsilon;
        }
    }
}
