using System;

namespace OpenMLTD.MilliSim.Core {
    public static class MathHelper {

        public static int Clamp(this int v, int min, int max) {
            return v < min ? min : (v > max ? max : v);
        }

        public static float Clamp(this float v, float min, float max) {
            return v < min ? min : (v > max ? max : v);
        }

        public static double Clamp(this double v, double min, double max) {
            return v < min ? min : (v > max ? max : v);
        }

        public static float Lerp(float from, float to, float percent) {
            return from * (1 - percent) + to * percent;
        }

        public static double Lerp(double from, double to, double percent) {
            return from * (1 - percent) + to * percent;
        }

        public static float LerpTo(this float from, float to, float percent) {
            return Lerp(from, to, percent);
        }

        public static double LerpTo(this double from, double to, double percent) {
            return Lerp(from, to, percent);
        }

        public static (double X, double Y) CubicBezier(double x1, double y1, double cx1, double cy1, double cx2, double cy2, double x2, double y2, double t) {
            t = Clamp(t, 0, 1);
            var tm = 1 - t;
            var tm2 = tm * tm;
            var tm3 = tm * tm2;
            var t2 = t * t;
            var t3 = t2 * t;
            var x = tm3 * x1 + 3 * tm2 * t * cx1 + 3 * tm * t2 * cx2 + t3 * x2;
            var y = tm3 * y1 + 3 * tm2 * t * cy1 + 3 * tm * t2 * cy2 + t3 * y2;
            return (x, y);
        }

        public static (float X, float Y) CubicBezier(float x1, float y1, float cx1, float cy1, float cx2, float cy2, float x2, float y2, float t) {
            t = Clamp(t, 0, 1);
            var tm = 1 - t;
            var tm2 = tm * tm;
            var tm3 = tm * tm2;
            var t2 = t * t;
            var t3 = t2 * t;
            var x = tm3 * x1 + 3 * tm2 * t * cx1 + 3 * tm * t2 * cx2 + t3 * x2;
            var y = tm3 * y1 + 3 * tm2 * t * cy1 + 3 * tm * t2 * cy2 + t3 * y2;
            return (x, y);
        }

        public static readonly Random Random = new Random();

    }
}
