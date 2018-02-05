namespace OpenMLTD.MilliSim.Core.Extensions {
    internal static class NumberExtensions {

        internal static int Clamp(this int v, int min, int max) {
            return v < min ? min : (v > max ? max : v);
        }

        internal static float Clamp(this float v, float min, float max) {
            return v < min ? min : (v > max ? max : v);
        }

        internal static double Clamp(this double v, double min, double max) {
            return v < min ? min : (v > max ? max : v);
        }

        internal static float ClampLower(this float v, float min) {
            return v < min ? min : v;
        }

    }
}
