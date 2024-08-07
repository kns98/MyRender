// Define the namespace for the Ray Tracer GUI application
namespace RayTracerGUI
{
    // Define a class to represent material properties
    public class Material
    {
        // Properties for color, reflectivity, emissivity, transparency, refractive index, and texture
        public Vector3f Color { get; set; }
        public double Reflectivity { get; set; }
        public double Emissivity { get; set; }
        public double Transparency { get; set; }
        public double RefractiveIndex { get; set; }
        public Texture Texture { get; set; }

        // Constructor to initialize material properties
        public Material(Vector3f color, double reflectivity, double emissivity, double transparency = 0.0, double refractiveIndex = 1.0, Texture texture = null)
        {
            Color = color;
            Reflectivity = reflectivity;
            Emissivity = emissivity;
            Transparency = transparency;
            RefractiveIndex = refractiveIndex;
            Texture = texture;
        }

        // Get the color of the material, considering texture if available
        public Vector3f GetColor(double u, double v)
        {
            return Texture != null ? Texture.GetColor(u, v) : Color;
        }
    }
}