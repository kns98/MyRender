// Define the namespace for the Ray Tracer GUI application
namespace RayTracerGUI
{
    public struct Vector3f
    {
        // Properties for the X, Y, and Z components of the vector
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        // Constructor to initialize the vector components
        public Vector3f(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        // Indexer to access vector components by index
        public double this[int index]
        {
            get
            {
                // Return the component based on the index provided
                double v = index switch
                {
                    0 => X,
                    1 => Y,
                    2 => Z,
                    _ => throw new IndexOutOfRangeException(),
                };
                return v;
            }
        }

        // Define some static properties for common vectors
        public static Vector3f Zero => new Vector3f(0, 0, 0);
        public static Vector3f OneX => new Vector3f(1, 0, 0);
        public static Vector3f OneY => new Vector3f(0, 1, 0);
        public static Vector3f OneZ => new Vector3f(0, 0, 1);

        // Overload operators for vector arithmetic
        public static Vector3f operator -(Vector3f a) => new Vector3f(-a.X, -a.Y, -a.Z);
        public static Vector3f operator -(Vector3f a, Vector3f b) => new Vector3f(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3f operator +(Vector3f a, Vector3f b) => new Vector3f(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3f operator *(Vector3f a, double scalar) => new Vector3f(a.X * scalar, a.Y * scalar, a.Z * scalar);
        public static Vector3f operator /(Vector3f a, double scalar) => new Vector3f(a.X / scalar, a.Y / scalar, a.Z / scalar);

        // Calculate the dot product of two vectors
        public static double Dot(Vector3f a, Vector3f b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        // Calculate the cross product of two vectors
        public static Vector3f Cross(Vector3f a, Vector3f b) => new Vector3f(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);

        // Normalize a vector to have a length of 1
        public static Vector3f Unitize(Vector3f vector)
        {
            double length = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
            return vector / length;
        }

        // Calculate the length (magnitude) of the vector
        public double Length() => Math.Sqrt(X * X + Y * Y + Z * Z);

        // Reflect a vector around a normal vector
        public static Vector3f Reflect(Vector3f incident, Vector3f normal)
        {
            return incident - normal * 2 * Dot(incident, normal);
        }

        // Refract a vector based on a normal and a refractive index
        public static Vector3f Refract(Vector3f incident, Vector3f normal, double eta)
        {
            double cosi = -Math.Max(-1.0, Math.Min(1.0, Dot(incident, normal)));
            double etai = 1, etat = eta;
            Vector3f n = normal;
            if (cosi < 0) { cosi = -cosi; etai = eta; etat = 1; n = -normal; }
            double etaRatio = etai / etat;
            double k = 1 - etaRatio * etaRatio * (1 - cosi * cosi);
            return k < 0 ? Zero : etaRatio * incident + (etaRatio * cosi - Math.Sqrt(k)) * n;
        }

        internal bool IsZero()
        {
            return X == 0 && Y == 0 && Z == 0;
        }
    }
}