// Define the namespace for the Ray Tracer GUI application
namespace RayTracerGUI
{
    public class BoundingBox
    {
        public BoundingBox(Vector3f small, Vector3f big)
        {
            Small = small;
            Big = big;
        }

        public Vector3f Small { get; }
        public Vector3f Big { get; }
    }
}