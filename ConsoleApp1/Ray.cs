// Define the namespace for the Ray Tracer GUI application
namespace RayTracerGUI
{
    // Define a class to represent a ray in 3D space
    public class Ray
    {
        public Vector3f Origin { get; }
        public Vector3f Direction { get; }

        // Constructor to initialize ray origin and direction
        public Ray(Vector3f origin, Vector3f direction)
        {
            Origin = origin;
            Direction = Vector3f.Unitize(direction);
        }

        // Check if the ray intersects with a triangle
        public bool Intersects(Triangle triangle, out double distance)
        {
            const double EPSILON = 0.0000001;
            Vector3f vertex0 = triangle.Vertex0;
            Vector3f vertex1 = triangle.Vertex1;
            Vector3f vertex2 = triangle.Vertex2;

            Vector3f edge1 = vertex1 - vertex0;
            Vector3f edge2 = vertex2 - vertex0;

            Vector3f h = Vector3f.Cross(Direction, edge2);
            double a = Vector3f.Dot(edge1, h);
            if (a > -EPSILON && a < EPSILON)
            {
                distance = 0;
                return false;
            }

            double f = 1.0 / a;
            Vector3f s = Origin - vertex0;
            double u = f * Vector3f.Dot(s, h);
            if (u < 0.0 || u > 1.0)
            {
                distance = 0;
                return false;
            }

            Vector3f q = Vector3f.Cross(s, edge1);
            double v = f * Vector3f.Dot(Direction, q);
            if (v < 0.0 || u + v > 1.0)
            {
                distance = 0;
                return false;
            }

            double t = f * Vector3f.Dot(edge2, q);
            if (t > EPSILON)
            {
                distance = t;
                return true;
            }
            else
            {
                distance = 0;
                return false;
            }
        }
    }
}