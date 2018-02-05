using System.Runtime.CompilerServices;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public static class FontHelper {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float PointsToPixels(float pt) {
            return pt * 4 / 3;
        }

    }
}
