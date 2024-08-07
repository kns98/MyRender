// Define the namespace for the Ray Tracer GUI application
namespace RayTracerGUI
{
    // Define a class to represent the scene
    public class Scene
    {
        private readonly List<Triangle> triangles;
        private readonly List<Light> lights;
        private BVHNode bvhRoot;

        // Constructor to initialize the scene
        public Scene()
        {
            triangles = new List<Triangle>();
            lights = new List<Light>();
        }

        // Add a triangle to the scene and update the BVH tree
        public void AddTriangle(Triangle triangle)
        {
            triangles.Add(triangle);
            bvhRoot = new BVHNode(triangles, 0, triangles.Count);
        }

        // Add a light to the scene
        public void AddLight(Light light)
        {
            lights.Add(light);
        }

        // Get all triangles in the scene
        public IEnumerable<Triangle> GetTriangles() => triangles;

        // Get all lights in the scene
        public IEnumerable<Light> GetLights() => lights;

        // Check if a ray intersects with any triangle in the scene
        public bool Intersect(Ray ray, out Triangle hitTriangle, out double hitDistance)
        {
            return bvhRoot.Intersect(ray, 0, double.MaxValue, out hitTriangle, out hitDistance);
        }
    }
}