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
