namespace MvConvex
{
    public class Edge : IEdge
    {
        public IVertex Start { get; }
        public IVertex End { get; }

        public Edge(IVertex start, IVertex end)
        {
            Start = start;
            End = end;
        }
    }
}
