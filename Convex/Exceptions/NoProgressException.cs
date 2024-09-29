namespace MvConvex
{
    public class NoProgressException : ConvexHullException
    {
        public NoProgressException() : base("Algorithm failed to make progress after multiple iterations.") { }
    }
}
