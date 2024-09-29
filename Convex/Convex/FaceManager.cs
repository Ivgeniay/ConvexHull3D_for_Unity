using System.Collections.Generic;
using System.Linq; 

namespace MvConvex
{
    public class FaceManager<TVertex, TTriangle, TFace>
        where TVertex : IVertex
        where TTriangle : class, ITriangle, new()
        where TFace : class, IFace<TVertex, TTriangle>, new()
    { 
        private readonly ILogger logger;

        public FaceManager(ILogger logger = null)
        {
            this.logger = logger;
        }

        public void UpdateFaceAdjacency(List<TFace> faces)
        {
            logger?.LogInformation($"Updating face adjacency for {faces.Count} faces.");
             
            int adjacencyCount = 0;
            foreach (var face in faces)
            {
                face.Adjacency = faces.Where(f => f != face && SharesEdge(face, f)).ToArray();
                adjacencyCount += face.Adjacency.Length;
            }

            logger?.LogInformation($"Face adjacency updated. Total adjacencies: {adjacencyCount}");
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
