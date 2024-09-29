using UnityEngine;

namespace MvConvex
{
    public class Triangle : ITriangle
    {
        public IVertex VertexA { get; private set; }
        public IVertex VertexB { get; private set; }
        public IVertex VertexC { get; private set; }
        public IEdge EdgeAB { get; private set; }
        public IEdge EdgeBC { get; private set; }
        public IEdge EdgeCA { get; private set; }
        private ITriangle AdjacentAB { get; set; }
        private ITriangle AdjacentBC { get; set; }
        private ITriangle AdjacentCA { get; set; }

        public IVertex[] FrontSideTriangle => null;
        public IVertex[] OtherSideTriangle => null;

        public Triangle() {  }

        public bool IsPointInFront(Vector3 point)
        {
            Vector3 normal = Vector3.Cross(VertexB.Position - VertexA.Position, VertexC.Position - VertexA.Position).normalized;
            return Vector3.Dot(point - VertexA.Position, normal) > 0;
        }

        public bool Contains(IVertex vertex)
        {
            return VertexA == vertex || VertexB == vertex || VertexC == vertex;
        }

        public bool HasEdge(IEdge edge)
        {
            return EdgeAB.Start == edge.Start && EdgeAB.End == edge.End ||
                   EdgeBC.Start == edge.Start && EdgeBC.End == edge.End ||
                   EdgeCA.Start == edge.Start && EdgeCA.End == edge.End ||
                   EdgeAB.End == edge.Start && EdgeAB.Start == edge.End ||
                   EdgeBC.End == edge.Start && EdgeBC.Start == edge.End ||
                   EdgeCA.End == edge.Start && EdgeCA.Start == edge.End;
        }

        public void SetAdjacentTriangles(ITriangle adjacentAB, ITriangle adjacentBC, ITriangle adjacentCA)
        {
            AdjacentAB = adjacentAB;
            AdjacentBC = adjacentBC;
            AdjacentCA = adjacentCA;
        }

        public void Initialize(IVertex a, IVertex b, IVertex c)
        {
            VertexA = a;
            VertexB = b;
            VertexC = c;
            EdgeAB = new Edge(a, b);
            EdgeBC = new Edge(b, c);
            EdgeCA = new Edge(c, a);
        }
    }
}
