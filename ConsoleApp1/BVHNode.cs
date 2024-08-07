using System.Drawing;

// Define the namespace for the Ray Tracer GUI application
namespace RayTracerGUI
{
    // Define a class for a bounding volume hierarchy (BVH) node
    public class BVHNode
    {
        public BVHNode Left { get; private set; }
        public BVHNode Right { get; private set; }
        public BoundingBox Box { get; private set; }
        public Triangle Triangle { get; private set; }

        // Constructor to create a BVH node from a list of triangles
        public BVHNode(List<Triangle> triangles, int start, int end)
        {
            var objects = new List<Triangle>(triangles);
            int axis = GetBestAxisToSplit(objects, start, end);
            objects.Sort((a, b) => a.GetBoundingBox().Small[axis].CompareTo(b.GetBoundingBox().Small[axis]));

            int objectSpan = end - start;

            if (objectSpan == 1)
            {
                Left = Right = new BVHNode(objects[start]);
            }
            else if (objectSpan == 2)
            {
                Left = new BVHNode(objects[start]);
                Right = new BVHNode(objects[start + 1]);
            }
            else
            {
                int mid = start + objectSpan / 2;
                Left = new BVHNode(objects, start, mid);
                Right = new BVHNode(objects, mid, end);
            }

            Box = BoundingBox.SurroundingBox(Left.Box, Right.Box);
        }

        // Determine the best axis to split the BVH node
        private int GetBestAxisToSplit(List<Triangle> triangles, int start, int end)
        {
            int bestAxis = 0;
            double bestCost = double.MaxValue;
            for (int axis = 0; axis < 3; axis++)
            {
                triangles.Sort((a, b) => a.GetBoundingBox().Small.[axis].CompareTo(b.GetBoundingBox().Small[axis]));
                BoundingBox boxLeft = triangles[start].GetBoundingBox();
                BoundingBox boxRight = triangles[end - 1].GetBoundingBox();
                double cost = 0;
                for (int i = start + 1; i < end; i++)
                {
                    boxLeft = BoundingBox.SurroundingBox(boxLeft, triangles[i].GetBoundingBox());
                    boxRight = BoundingBox.SurroundingBox(boxRight, triangles[i].GetBoundingBox());
                    double surfaceAreaLeft = boxLeft.GetSurfaceArea();
                    double surfaceAreaRight = boxRight.GetSurfaceArea();
                    cost = surfaceAreaLeft * (i - start) + surfaceAreaRight * (end - i);
                }
                if (cost < bestCost)
                {
                    bestAxis = axis;
                    bestCost = cost;
                }
            }
            return bestAxis;
        }

        // Constructor to create a leaf BVH node for a single triangle
        private BVHNode(Triangle triangle)
        {
            Triangle = triangle;
            Box = triangle.GetBoundingBox();
        }

        // Check if a ray intersects with the BVH node
        public bool Intersect(Ray ray, double tMin, double tMax, out Triangle hitTriangle, out double hitDistance)
        {
            // Initialize output variables
            hitTriangle = null;
            hitDistance = double.MaxValue;

            // Check intersection with the bounding box
            if (!Box.Intersects(ray, tMin, tMax))
            {
                Console.WriteLine("No intersection with bounding box.");
                return false;
            }

            // Attempt to intersect with the left and right child nodes
            bool hitLeft = Left?.Intersect(ray, tMin, tMax, out Triangle leftTriangle, out double leftDistance) ?? false;
            bool hitRight = Right?.Intersect(ray, tMin, tMax, out Triangle rightTriangle, out double rightDistance) ?? false;

            // Debug output
            Console.WriteLine($"Hit Left: {hitLeft}, Left Distance: {leftDistance}");
            Console.WriteLine($"Hit Right: {hitRight}, Right Distance: {rightDistance}");

            // Determine which intersection is closer and set the output accordingly
            if (hitLeft && hitRight)
            {
                if (leftDistance < rightDistance)
                {
                    hitTriangle = leftTriangle;
                    hitDistance = leftDistance;
                }
                else
                {
                    hitTriangle = rightTriangle;
                    hitDistance = rightDistance;
                }
                Console.WriteLine($"Both left and right hit. Closer hit: {hitDistance}");
                return true;
            }

            if (hitLeft)
            {
                hitTriangle = leftTriangle;
                hitDistance = leftDistance;
                Console.WriteLine($"Only left hit. Distance: {hitDistance}");
                return true;
            }

            if (hitRight)
            {
                hitTriangle = rightTriangle;
                hitDistance = rightDistance;
                Console.WriteLine($"Only right hit. Distance: {hitDistance}");
                return true;
            }

            // No intersection found
            Console.WriteLine("No intersection found with either child.");
            return false;
        }


        // Define a class for a bounding box
        public class BoundingBox
        {
            public Vector3f Min { get; private set; }
            public Vector3f Max { get; private set; }

            // Constructor to initialize the bounding box
            public BoundingBox(Vector3f min, Vector3f max)
            {
                Min = min;
                Max = max;
            }

            // Create a bounding box that surrounds two other bounding boxes
            public static BoundingBox SurroundingBox(BoundingBox box0, BoundingBox box1)
            {
                Vector3f small = new Vector3f(Math.Min(box0.Min.X, box1.Min.X),
                                              Math.Min(box0.Min.Y, box1.Min.Y),
                                              Math.Min(box0.Min.Z, box1.Min.Z));

                Vector3f big = new Vector3f(Math.Max(box0.Max.X, box1.Max.X),
                                            Math.Max(box0.Max.Y, box1.Max.Y),
                                            Math.Max(box0.Max.Z, box1.Max.Z));

                return new BoundingBox(small, big);
            }

            // Check if a ray intersects with the bounding box
            public bool Intersects(Ray ray, double tMin, double tMax)
            {
                for (int a = 0; a < 3; a++)
                {
                    double invD = 1.0 / ray.Direction[a];
                    double t0 = (Min[a] - ray.Origin[a]) * invD;
                    double t1 = (Max[a] - ray.Origin[a]) * invD;

                    if (invD < 0.0)
                    {
                        double temp = t0;
                        t0 = t1;
                        t1 = temp;
                    }

                    tMin = t0 > tMin ? t0 : tMin;
                    tMax = t1 < tMax ? t1 : tMax;

                    if (tMax <= tMin)
                        return false;
                }
                return true;
            }

            // Calculate the surface area of the bounding box
            public double GetSurfaceArea()
            {
                Vector3f diff = Max - Min;
                return 2 * (diff.X * diff.Y + diff.X * diff.Z + diff.Y * diff.Z);
            }
        }

        // Define a class for the ray tracer
        public class RayTracer
        {
            private readonly Scene scene;
            private readonly int maxDepth;
            private readonly int samplesPerPixel;

            // Constructor to initialize the ray tracer with a scene, max depth, and samples per pixel
            public RayTracer(Scene scene, int maxDepth = 5, int samplesPerPixel = 100)
            {
                this.scene = scene;
                this.maxDepth = maxDepth;
                this.samplesPerPixel = samplesPerPixel;
            }

            // Calculate the radiance at a given position and direction
            public Vector3f Radiance(Vector3f position, Vector3f direction, Random random, int depth = 0)
            {
                if (depth > maxDepth)
                    return Vector3f.Zero;

                Ray ray = new Ray(position, direction);
                if (scene.Intersect(ray, out Triangle hitTriangle, out double hitDistance))
                {
                    Vector3f hitPoint = ray.Origin + ray.Direction * hitDistance;
                    Material material = hitTriangle.GetMaterial();
                    Vector3f normal = hitTriangle.Normal();
                    double u, v;
                    hitTriangle.Intersect(ray, out _, out u, out v);
                    Vector3f color = material.Emissivity * material.GetColor(u, v);

                    Vector3f directLighting = Vector3f.Zero;
                    foreach (var light in scene.GetLights())
                    {
                        Vector3f lightDirection = Vector3f.Unitize(light.Position - hitPoint);
                        Ray shadowRay = new Ray(hitPoint + normal * 0.001, lightDirection);

                        if (!scene.Intersect(shadowRay, out _, out _))
                        {
                            double lambertian = Math.Max(0, Vector3f.Dot(normal, lightDirection));
                            Vector3f reflectionDirection = Vector3f.Reflect(-lightDirection, normal);
                            double specular = Math.Pow(Math.Max(0, Vector3f.Dot(reflectionDirection, direction)), 32);

                            directLighting += light.Color * (material.GetColor(u, v) * lambertian + specular * material.Reflectivity);
                        }
                    }

                    Vector3f indirectLighting = Vector3f.Zero;
                    for (int i = 0; i < samplesPerPixel; i++)
                    {
                        Vector3f randomDirection = RandomHemisphereDirection(normal, random);
                        indirectLighting += Radiance(hitPoint + normal * 0.001, randomDirection, random, depth + 1);
                    }
                    indirectLighting /= samplesPerPixel;

                    color += directLighting + indirectLighting * material.Color;
                    return color;
                }

                return Vector3f.Zero;
            }

            // Generate a random direction within a hemisphere
            private Vector3f RandomHemisphereDirection(Vector3f normal, Random random)
            {
                double u = random.NextDouble();
                double v = random.NextDouble();
                double theta = 2 * Math.PI * u;
                double phi = Math.Acos(2 * v - 1);
                double x = Math.Sin(phi) * Math.Cos(theta);
                double y = Math.Sin(phi) * Math.Sin(theta);
                double z = Math.Cos(phi);
                Vector3f randomDirection = new Vector3f(x, y, z);
                return Vector3f.Dot(randomDirection, normal) > 0 ? randomDirection : -randomDirection;
            }
        }

        // Define a class for the rendered image
        public class RenderedImage
        {
            private readonly int width;
            private readonly int height;
            private readonly Vector3f[,] pixels;

            // Constructor to initialize the rendered image with dimensions
            public RenderedImage(int width, int height)
            {
                this.width = width;
                this.height = height;
                pixels = new Vector3f[width, height];
            }

            // Properties for width, height, and aspect ratio
            public int Width => width;
            public int Height => height;
            public double AspectRatio => (double)width / height;

            // Add a color to a specific pixel
            public void AddToPixel(int x, int y, Vector3f color)
            {
                pixels[x, y] = color;
            }

            // Get the color of a specific pixel
            public Vector3f GetPixel(int x, int y)
            {
                return pixels[x, y];
            }

            // Save the rendered image as a PPM file
            public void SaveAsPPM(string filename)
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    writer.WriteLine($"P3\n{width} {height}\n255");
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            Vector3f color = pixels[x, y];
                            int r = (int)(Math.Min(1.0, color.X) * 255);
                            int g = (int)(Math.Min(1.0, color.Y) * 255);
                            int b = (int)(Math.Min(1.0, color.Z) * 255);
                            writer.WriteLine($"{r} {g} {b}");
                        }
                    }
                }
            }
        }

        // Define a static helper class for parallel processing
        public static class ParallelHelper
        {
            // Execute an action for each element in a 2D range
            public static void For2D(int startY, int endY, int startX, int endX, IAction2D action)
            {
                int batchHeight = (endY - startY) / Environment.ProcessorCount;

                Parallel.For(0, Environment.ProcessorCount, i =>
                {
                    int currentStartY = startY + i * batchHeight;
                    int currentEndY = (i == Environment.ProcessorCount - 1) ? endY : currentStartY + batchHeight;

                    for (int y = currentStartY; y <= currentEndY; y++)
                    {
                        for (int x = startX; x <= endX; x++)
                        {
                            action.Invoke(x, y);
                        }
                    }
                });
            }
        }

        // Define an interface for a 2D action
        public interface IAction2D
        {
            void Invoke(int i, int j);
        }

        // Define a class for the camera
        public class Camera
        {
            private readonly Vector3f viewPosition;
            private readonly double viewAngle;
            private readonly Vector3f viewDirection;
            private readonly Vector3f right;
            private readonly Vector3f up;

            // Constructor to initialize the camera from an input buffer
            public Camera(TextReader inBuffer)
            {
                viewPosition = Scanf.Read(inBuffer);
                viewDirection = Vector3f.Unitize(Scanf.Read(inBuffer));
                viewDirection = viewDirection.IsZero() ? Vector3f.OneZ : viewDirection;

                string line = Scanf.GetLine(inBuffer);
                double angle = Math.Max(10, Math.Min(160, double.Parse(line)));
                viewAngle = angle * (Math.PI / 180.0);

                right = Vector3f.Unitize(Vector3f.Cross(Vector3f.OneY, viewDirection));
                if (right.IsZero())
                {
                    right = Vector3f.Unitize(Vector3f.Cross(viewDirection, Vector3f.OneX));
                }

                up = Vector3f.Unitize(Vector3f.Cross(viewDirection, right));
            }

            // Property for the camera's eye point (position)
            public Vector3f EyePoint => viewPosition;

            // Render a frame of the scene
            public RenderedImage Frame(Scene scene, RenderedImage renderedImage, Random random)
            {
                var rayTracer = new RayTracer(scene);
                var action = new Render2d(rayTracer, renderedImage, new Random(), up, right, viewDirection, viewAngle, viewPosition);

                ParallelHelper.For2D(0, renderedImage.Height - 1, 0, renderedImage.Width - 1, action);

                return renderedImage;
            }

            // Define a nested class to render a 2D image
            private class Render2d : IAction2D
            {
                private readonly RayTracer rayTracer;
                private readonly RenderedImage renderedImage;
                private readonly Random rand;
                private readonly Vector3f up;
                private readonly Vector3f right;
                private readonly Vector3f dir;
                private readonly double ang;
                private readonly Vector3f pos;
                private int count;
                private readonly int total;
                private readonly int samplesPerPixel;

                // Constructor to initialize the render action
                public Render2d(RayTracer r, RenderedImage i, Random ra, Vector3f up, Vector3f right, Vector3f dir, double ang, Vector3f pos, int samplesPerPixel = 4)
                {
                    rayTracer = r;
                    renderedImage = i;
                    rand = ra;
                    this.up = up;
                    this.right = right;
                    this.dir = dir;
                    this.ang = ang;
                    this.pos = pos;
                    this.samplesPerPixel = samplesPerPixel;
                    total = (renderedImage.Height - 1) * (renderedImage.Width - 1);
                }

                // Execute the rendering action for each pixel
                public void Invoke(int x, int y)
                {
                    Vector3f color = Vector3f.Zero;

                    for (int i = 0; i < samplesPerPixel; i++)
                    {
                        double u = (x + rand.NextDouble()) / renderedImage.Width;
                        double v = (y + rand.NextDouble()) / renderedImage.Height;

                        double f1 = (u * 2 - 1) * Math.Tan(ang * 0.5) * renderedImage.AspectRatio;
                        double num1 = (v * 2 - 1) * Math.Tan(ang * 0.5);

                        var V_2 = right * f1 + up * num1;
                        var V_3 = Vector3f.Unitize(dir + V_2);
                        color += rayTracer.Radiance(pos, V_3, rand);
                    }

                    color /= samplesPerPixel;
                    renderedImage.AddToPixel(x, y, color);

                    int num2 = Interlocked.Increment(ref count);
                    if (num2 % 100000 == 0)
                    {
                        int V_7 = num2 / (total / 100);
                        Console.WriteLine($"{V_7} % of pixels processed: {DateTime.Now}");
                    }
                }
            }
        }

        // Define a static helper class for input parsing
        public static class Scanf
        {
            // Read a line from a text reader
            public static string GetLine(TextReader reader)
            {
                return reader.ReadLine();
            }

            // Read a vector from a text reader
            public static Vector3f Read(TextReader reader)
            {
                string[] parts = reader.ReadLine().Split();
                return new Vector3f(double.Parse(parts[0]), double.Parse(parts[1]), double.Parse(parts[2]));
            }
        }

        // Define a partial class for the main form
        public partial class Program
        {
            private Scene scene;
            private Camera camera;
            private RenderedImage renderedImage;
            private RayTracer rayTracer;

            // Constructor to initialize the main form and scene
            public static void Main(string[] args)
            {
                InitializeScene();
            }

            // Initialize the scene with a default setup
            private void InitializeScene()
            {
                int width = 800;
                int height = 600;
                renderedImage = new RenderedImage(width, height);
                scene = TestScene.InitializeScene();
                scene.AddTriangle(new Triangle(new Vector3f(0, -1, 3), new Vector3f(1, 1, 3), new Vector3f(-1, 1, 3), new Material(new Vector3f(0.8, 0.3, 0.3), 0.5, 0.1), new Vector3f(0, 0), new Vector3f(1, 0), new Vector3f(0, 1)));
                scene.AddLight(new Light(new Vector3f(0, 5, 5), new Vector3f(1, 1, 1)));

                using (TextReader reader = new StringReader("0 0 0\n0 0 1\n90"))
                {
                    camera = new Camera(reader);
                }

                rayTracer = new RayTracer(scene);
            }

            // Event handler for the render button click event
            private async void RenderButton_Click(object sender, EventArgs e)
            {
                RenderButton.Enabled = false;
                await Task.Run(() => camera.Frame(scene, renderedImage, new Random()));
                RenderButton.Enabled = true;
                DisplayRenderedImage();
            }

            // Display the rendered image in a picture box
            private void DisplayRenderedImage()
            {
                Bitmap bitmap = new Bitmap(renderedImage.Width, renderedImage.Height);
                for (int y = 0; y < renderedImage.Height; y++)
                {
                    for (int x = 0; x < renderedImage.Width; x++)
                    {
                        Vector3f color = renderedImage.GetPixel(x, y);
                        int r = (int)(Math.Min(1.0, color.X) * 255);
                        int g = (int)(Math.Min(1.0, color.Y) * 255);
                        int b = (int)(Math.Min(1.0, color.Z) * 255);
                        bitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                    }
                }

                RenderedPictureBox.Image = bitmap;
            }
        }
    }
}
