using System.Collections.Generic; 

namespace MvConvex
{
    public interface IConvexHullResult<TVertex> 
    where TVertex : IVertex
    {
        public IReadOnlyList<TVertex> Vertices { get; }
        public void SetConvexHull(IEnumerable<TVertex> hullVertices);
    }

    public interface IConvexHullResult<TVertex, TTriangle>
    where TVertex : IVertex
    where TTriangle : ITriangle
    {
        IReadOnlyList<TVertex> HullVertices { get; }
        IReadOnlyList<TTriangle> HullTriangles { get; }
        IReadOnlyList<TVertex> InteriorVertices { get; }
        void SetConvexHullData(IEnumerable<TVertex> hullVertices, IEnumerable<TTriangle> hullTriangles, IEnumerable<TVertex> interiorVertices);
    }

    public interface IConvexHullResult<TVertex, TTriangle, TFace>
    where TVertex : IVertex
    where TTriangle : ITriangle
    where TFace : IFace<TVertex, TTriangle>
    {
        IReadOnlyList<TVertex> HullVertices { get; }
        IReadOnlyList<TTriangle> HullTriangles { get; }
        IReadOnlyList<TFace> HullFaces { get; }
        IReadOnlyList<TVertex> InteriorVertices { get; }
        void SetConvexHullData(IEnumerable<TVertex> hullVertices, IEnumerable<TTriangle> hullTriangles, IEnumerable<TFace> hullFaces, IEnumerable<TVertex> interiorVertices);
    }

}
