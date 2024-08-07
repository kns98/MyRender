using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// Define the namespace for the Ray Tracer GUI application
namespace RayTracerGUI
{
    // Define a structure to represent a 3D vector

    public static class TestScene
    {
        // Initialize the scene with a default setup, including a cube
        public static Scene InitializeScene()
        {
            int width = 800;
            int height = 600;
            var renderedImage = new RenderedImage(width, height);
            var scene = new Scene();

            // Define material for the cube
            Material cubeMaterial = new Material(new Vector3f(0.3, 0.7, 0.3), 0.5, 0.1);

            // Define cube vertices
            Vector3f v0 = new Vector3f(-1, -1, 2);
            Vector3f v1 = new Vector3f(1, -1, 2);
            Vector3f v2 = new Vector3f(1, 1, 2);
            Vector3f v3 = new Vector3f(-1, 1, 2);
            Vector3f v4 = new Vector3f(-1, -1, 4);
            Vector3f v5 = new Vector3f(1, -1, 4);
            Vector3f v6 = new Vector3f(1, 1, 4);
            Vector3f v7 = new Vector3f(-1, 1, 4);

            // Add cube faces to the scene (each face consists of 2 triangles)
            // Front face
            scene.AddTriangle(new Triangle(v0, v1, v2, cubeMaterial, Vector3f.Zero, Vector3f.OneX, Vector3f.OneY));
            scene.AddTriangle(new Triangle(v0, v2, v3, cubeMaterial, Vector3f.Zero, Vector3f.OneY, Vector3f.OneX));

            // Back face
            scene.AddTriangle(new Triangle(v4, v5, v6, cubeMaterial, Vector3f.Zero, Vector3f.OneX, Vector3f.OneY));
            scene.AddTriangle(new Triangle(v4, v6, v7, cubeMaterial, Vector3f.Zero, Vector3f.OneY, Vector3f.OneX));

            // Left face
            scene.AddTriangle(new Triangle(v0, v3, v7, cubeMaterial, Vector3f.Zero, Vector3f.OneX, Vector3f.OneY));
            scene.AddTriangle(new Triangle(v0, v7, v4, cubeMaterial, Vector3f.Zero, Vector3f.OneY, Vector3f.OneX));

            // Right face
            scene.AddTriangle(new Triangle(v1, v2, v6, cubeMaterial, Vector3f.Zero, Vector3f.OneX, Vector3f.OneY));
            scene.AddTriangle(new Triangle(v1, v6, v5, cubeMaterial, Vector3f.Zero, Vector3f.OneY, Vector3f.OneX));

            // Top face
            scene.AddTriangle(new Triangle(v2, v3, v7, cubeMaterial, Vector3f.Zero, Vector3f.OneX, Vector3f.OneY));
            scene.AddTriangle(new Triangle(v2, v7, v6, cubeMaterial, Vector3f.Zero, Vector3f.OneY, Vector3f.OneX));

            // Bottom face
            scene.AddTriangle(new Triangle(v0, v1, v5, cubeMaterial, Vector3f.Zero, Vector3f.OneX, Vector3f.OneY));
            scene.AddTriangle(new Triangle(v0, v5, v4, cubeMaterial, Vector3f.Zero, Vector3f.OneY, Vector3f.OneX));

            // Add a light to the scene
            scene.AddLight(new Light(new Vector3f(0, 5, 5), new Vector3f(1, 1, 1)));

            return scene;
        }
    }
}
