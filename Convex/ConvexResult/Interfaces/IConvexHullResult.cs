using System.Collections.Generic; 

namespace MvConvex
{
    /// <summary>
    /// Интерфейс для результата построения выпуклой оболочки.
    /// </summary>
    public interface IConvexHullResult<TVertex, TTriangle, TFace, TEdge>
        where TVertex : IVertex
        where TTriangle : ITriangle
        where TFace : IFace<TVertex, TTriangle>
        where TEdge : IEdge
    {
        /// <summary>Вершины, формирующие выпуклую оболочку.</summary>
        IReadOnlyList<TVertex> HullVertices { get; }

        /// <summary>Треугольники, формирующие выпуклую оболочку.</summary>
        IReadOnlyList<TTriangle> HullTriangles { get; }

        /// <summary>Грани выпуклой оболочки.</summary>
        IReadOnlyList<TFace> HullFaces { get; }

        /// <summary>Ребра выпуклой оболочки.</summary>
        IReadOnlyList<TEdge> HullEdges { get; }

        /// <summary>Вершины, находящиеся внутри выпуклой оболочки.</summary>
        IReadOnlyList<TVertex> InteriorVertices { get; }

        /// <summary>Вершины, не использованные из-за их близости к другим вершинам.</summary>
        IReadOnlyList<TVertex> IgnoredVertices { get; }

        /// <summary>Устанавливает данные результата построения выпуклой оболочки.</summary>
        void SetConvexHullData(
            IEnumerable<TVertex> hullVertices,
            IEnumerable<TTriangle> hullTriangles,
            IEnumerable<TFace> hullFaces,
            IEnumerable<TEdge> hullEdges,
            IEnumerable<TVertex> interiorVertices,
            IEnumerable<TVertex> ignoredVertices);
    }

}
