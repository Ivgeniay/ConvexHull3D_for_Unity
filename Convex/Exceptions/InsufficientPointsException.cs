namespace MvConvex
{
    public class InsufficientPointsException : ConvexHullException
    {
        public InsufficientPointsException() : base("At least 4 non-coplanar points are required to construct a 3D convex hull.") { }
    }
}
