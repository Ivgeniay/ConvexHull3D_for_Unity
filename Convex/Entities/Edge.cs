namespace MvConvex
{
    public class Edge : IEdge
    {
        public IVertex Start { get; private set; }
        public IVertex End { get; private set; }

        public Edge() { }
        public Edge(IVertex start, IVertex end)
        {
            Start = start;
            End = end;
        }
        public void Initialize(IVertex start, IVertex end)
        {
            Start = start;
            End = end;
        }
        public override bool Equals(object obj)
        {
            if (obj is IEdge other)
            {
                return (Start == other.Start && End == other.End) || (Start == other.End && End == other.Start);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() ^ End.GetHashCode();
        }
    }
}
