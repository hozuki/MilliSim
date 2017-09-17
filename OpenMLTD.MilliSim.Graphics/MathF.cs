using System;
using SharpDX;

namespace OpenMLTD.MilliSim.Graphics {
    public static class MathF {

        static MathF() {
            Rand = new Random();
        }

        public const float PI = (float)Math.PI;

        public static float Sin(float a) {
            return (float)Math.Sin(a);
        }

        public static float Cos(float a) {
            return (float)Math.Cos(a);
        }

        public static float Tan(float a) {
            return (float)Math.Tan(a);
        }

        public static float Acos(float f) {
            return (float)Math.Acos(f);
        }

        public static float Atan(float f) {
            return (float)Math.Atan(f);
        }

        public static float Atan2(float y, float x) {
            return (float)Math.Atan2(y, x);
        }

        public static float Sqrt(float f) {
            return (float)Math.Sqrt(f);
        }

        public static float Clamp(float value, float min, float max) {
            return value < min ? min : (value > max ? max : value);
        }

        public static Matrix InverseTranspose(Matrix m) {
            m.M41 = m.M42 = m.M43 = 0;
            m.M44 = 1;
            return Matrix.Transpose(Matrix.Invert(m));
        }

        public static readonly Random Rand;

        public static readonly float Sqrt2 = (float)Math.Sqrt(2);

        public static Vector3 Maximize(Vector3 v1, Vector3 v2) {
            return new Vector3(Math.Max(v1.X, v2.X), Math.Max(v1.Y, v2.Y), Math.Max(v1.Z, v2.Z));
        }

        public static Vector3 Minimize(Vector3 v1, Vector3 v2) {
            return new Vector3(Math.Min(v1.X, v2.X), Math.Min(v1.Y, v2.Y), Math.Min(v1.Z, v2.Z));
        }

        public static float AngleFromXY(float x, float y) {
            float theta;
            if (x >= 0.0f) {
                theta = Atan(y / x);
                if (theta < 0.0f) {
                    theta += 2 * PI;
                }
            } else {
                theta = Atan(y / x) + PI;
            }
            return theta;
        }

    }
}
