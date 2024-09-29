namespace MvConvex
{
    public class DegenerateTetrahedronException : ConvexHullException
    {
        public DegenerateTetrahedronException() : base("Unable to create a non-degenerate tetrahedron.") { }
    }
}
