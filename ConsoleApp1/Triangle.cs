// Define the namespace for the Ray Tracer GUI application
namespace RayTracerGUI
{
    // Define a class to represent a triangle in 3D space
    public class Triangle
    {
        public Vector3f Vertex0 { get; }
        public Vector3f Vertex1 { get; }
        public Vector3f Vertex2 { get; }
        private readonly Material material;
        private readonly Vector3f uv0;
        private readonly Vector3f uv1;
        private readonly Vector3f uv2;

        // Constructor to initialize triangle vertices, material, and UV coordinates
        public Triangle(Vector3f vertex0, Vector3f vertex1, Vector3f vertex2, Material material, Vector3f uv0, Vector3f uv1, Vector3f uv2)
        {
            Vertex0 = vertex0;
            Vertex1 = vertex1;
            Vertex2 = vertex2;
            this.material = material;
            this.uv0 = uv0;
            this.uv1 = uv1;
            this.uv2 = uv2;
        }

        // Get the material of the triangle
        public Material GetMaterial()
        {
            return material;
        }

        // Calculate the normal vector of the triangle
        public Vector3f Normal()
        {
            return Vector3f.Unitize(Vector3f.Cross(Vertex1 - Vertex0, Vertex2 - Vertex0));
        }

        // Get the bounding box of the triangle
        public BoundingBox GetBoundingBox()
        {
            Vector3f small = new Vector3f(
                Math.Min(Vertex0.X, Math.Min(Vertex1.X, Vertex2.X)),
                Math.Min(Vertex0.Y, Math.Min(Vertex1.Y, Vertex2.Y)),
                Math.Min(Vertex0.Z, Math.Min(Vertex1.Z, Vertex2.Z))
            );

            Vector3f big = new Vector3f(
                Math.Max(Vertex0.X, Math.Max(Vertex1.X, Vertex2.X)),
                Math.Max(Vertex0.Y, Math.Max(Vertex1.Y, Vertex2.Y)),
                Math.Max(Vertex0.Z, Math.Max(Vertex1.Z, Vertex2.Z))
            );

            return new BoundingBox(small, big);
        }

        // Check if a ray intersects with the triangle, and get intersection details
        public bool Intersect(Ray ray, out double distance, out double u, out double v)
        {
            if (ray.Intersects(this, out distance))
            {
                Vector3f p = ray.Origin + ray.Direction * distance;
                Vector3f w = p - Vertex0;
                double denominator = Vector3f.Dot(Vector3f.Cross(Vertex1 - Vertex0, Vertex2 - Vertex0), Normal());
                u = Vector3f.Dot(Vector3f.Cross(Vertex2 - Vertex0, w), Normal()) / denominator;
                v = Vector3f.Dot(Vector3f.Cross(w, Vertex1 - Vertex0), Normal()) / denominator;
                return true;
            }
            u = v = 0;
            return false;
        }
    }
}