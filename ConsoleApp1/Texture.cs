// Define the namespace for the Ray Tracer GUI application
namespace RayTracerGUI
{
    // Define a class for textures
    public class Texture
    {
        // Array to store pixel colors
        private readonly Vector3f[,] pixels;
        public int Width { get; }
        public int Height { get; }

        // Constructor to load texture from a file
        public Texture(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                reader.ReadLine(); // P3
                string[] dimensions = reader.ReadLine().Split();
                Width = int.Parse(dimensions[0]);
                Height = int.Parse(dimensions[1]);
                reader.ReadLine(); // 255

                pixels = new Vector3f[Width, Height];
                for (int j = 0; j < Height; j++)
                {
                    for (int i = 0; i < Width; i++)
                    {
                        int r = int.Parse(reader.ReadLine());
                        int g = int.Parse(reader.ReadLine());
                        int b = int.Parse(reader.ReadLine());
                        pixels[i, j] = new Vector3f(r / 255.0, g / 255.0, b / 255.0);
                    }
                }
            }
        }

        // Get the color at specified texture coordinates
        public Vector3f GetColor(double u, double v)
        {
            int i = (int)((u * Width) % Width);
            int j = (int)(((1 - v) * Height) % Height);
            return pixels[i, j];
        }
    }
}