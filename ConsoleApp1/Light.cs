// Define the namespace for the Ray Tracer GUI application
namespace RayTracerGUI
{
    // Define a class for a light source
    public class Light
    {
        public Vector3f Position { get; set; }
        public Vector3f Color { get; set; }

        // Constructor to initialize light position and color
        public Light(Vector3f position, Vector3f color)
        {
            Position = position;
            Color = color;
        }
    }
}
