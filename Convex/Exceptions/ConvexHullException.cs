using System; 

namespace MvConvex
{
    public class ConvexHullException : Exception
    {
        public ConvexHullException(string message) : base(message) { }
        public ConvexHullException(string message, Exception innerException) : base(message, innerException) { }
    }
}
