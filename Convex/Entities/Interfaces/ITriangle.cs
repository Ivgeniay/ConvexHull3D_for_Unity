using UnityEngine;

namespace MvConvex
{
    public interface ITriangle
    {
        public IVertex VertexA { get; }
        public IVertex VertexB { get; }
        public IVertex VertexC { get; }

        public IEdge EdgeAB { get; }
        public IEdge EdgeBC { get; }
        public IEdge EdgeCA { get; }

        public IVertex[] FrontSideTriangle { get; }
        public IVertex[] OtherSideTriangle { get; }

        public void Initialize(IVertex a, IVertex b, IVertex c);
        public bool IsPointInFront(Vector3 point);
        public bool Contains(IVertex vertex);
        public bool HasEdge(IEdge edge);
        public void SetAdjacentTriangles(ITriangle adjacentAB, ITriangle adjacentBC, ITriangle adjacentCA);
    }
}
