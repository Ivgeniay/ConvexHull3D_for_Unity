

using UnityEngine;

namespace MvConvex
{
    public class Face<TVertex, TTriangle> : IFace<TVertex, TTriangle>
        where TVertex : IVertex
        where TTriangle : ITriangle
    {
        public IFace<TVertex, TTriangle>[] Adjacency { get; set; }
        public ITriangle[] Triangles { get; private set; }
        public Vector3 Normal { get; set; }

        public void Initialize(ITriangle[] triangles)
        {
            Triangles = triangles;
            CalculateNormal();
        }

        private void CalculateNormal()
        {
            if (Triangles.Length > 0)
            {
                var triangle = Triangles[0];
                Vector3 v1 = triangle.VertexB.Position - triangle.VertexA.Position;
                Vector3 v2 = triangle.VertexC.Position - triangle.VertexA.Position;
                Normal = Vector3.Cross(v1, v2).normalized;
            }
        }
    }
}
