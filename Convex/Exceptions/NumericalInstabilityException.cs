namespace MvConvex
{
    public class NumericalInstabilityException : ConvexHullException
    {
        public NumericalInstabilityException(string message) : base(message) { }
    }
}
