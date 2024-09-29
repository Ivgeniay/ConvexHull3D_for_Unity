using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MvConvex
{  
    public class DefaultConvexHullResult<TVertex, TTriangle, TFace> : IConvexHullResult<TVertex, TTriangle, TFace>
    where TVertex : IVertex
    where TTriangle : ITriangle
    where TFace : IFace<TVertex, TTriangle>
    {
        public IReadOnlyList<TVertex> HullVertices { get; private set; }
        public IReadOnlyList<TTriangle> HullTriangles { get; private set; }
        public IReadOnlyList<TFace> HullFaces { get; private set; }
        public IReadOnlyList<TVertex> InteriorVertices { get; private set; }

        public void SetConvexHullData(IEnumerable<TVertex> hullVertices, IEnumerable<TTriangle> hullTriangles, IEnumerable<TFace> hullFaces, IEnumerable<TVertex> interiorVertices)
        {
            HullVertices = hullVertices.ToList();
            HullTriangles = hullTriangles.ToList();
            HullFaces = hullFaces.ToList();
            InteriorVertices = interiorVertices.ToList();
        }

        public Mesh CreateMeshFromResult()
        {
            Mesh mesh = new Mesh();
             
            List<Vector3> meshVertices = new List<Vector3>();
            List<int> meshTriangles = new List<int>();
             
            Dictionary<TVertex, int> vertexToIndex = new Dictionary<TVertex, int>();
            for (int i = 0; i < HullVertices.Count; i++)
            {
                meshVertices.Add(HullVertices[i].Position);
                vertexToIndex[HullVertices[i]] = i;
            }
             
            foreach (var triangle in HullTriangles)
            {
                meshTriangles.Add(vertexToIndex[(TVertex)triangle.VertexA]);
                meshTriangles.Add(vertexToIndex[(TVertex)triangle.VertexB]);
                meshTriangles.Add(vertexToIndex[(TVertex)triangle.VertexC]);
            }
             
            mesh.SetVertices(meshVertices);
            mesh.SetTriangles(meshTriangles, 0);
             
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}
