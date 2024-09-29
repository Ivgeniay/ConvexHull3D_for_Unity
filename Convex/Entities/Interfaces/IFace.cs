using UnityEngine;

namespace MvConvex
{
    public interface IFace<TVertex, TTriangle>
        where TVertex : IVertex
        where TTriangle : ITriangle
    {
        IFace<TVertex, TTriangle>[] Adjacency { get; set; }
        ITriangle[] Triangles { get; }
        Vector3 Normal { get; set; }
        void Initialize(ITriangle[] triangles);
    }
}
