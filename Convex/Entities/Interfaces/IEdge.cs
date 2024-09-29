namespace MvConvex
{
    public interface IEdge
    {
        public IVertex Start { get; }
        public IVertex End { get; }

        public void Initialize(IVertex start, IVertex end);
    }
}
