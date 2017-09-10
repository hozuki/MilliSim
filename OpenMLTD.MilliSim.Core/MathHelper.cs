namespace OpenMLTD.MilliSim.Core {
    public static class MathHelper {

        public static int Clamp(this int v, int min, int max) {
            return v < min ? min : (v > max ? max : v);
        }

        public static float Clamp(this float v, float min, float max) {
            return v < min ? min : (v > max ? max : v);
        }

        public static float Lerp(float from, float to, float percent) {
            return from * (1 - percent) + to * percent;
        }

        public static float LerpTo(this float from, float to, float percent) {
            return Lerp(from, to, percent);
        }

    }
}
