using System;
using System.Runtime.CompilerServices;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    /// <summary>
    /// Font helper functions.
    /// </summary>
    public static class FontHelper {

        /// <summary>
        /// Converts the size of font from points (pt) to pixels (px).
        /// </summary>
        /// <param name="pt">Font size in points.</param>
        /// <returns>Font size in pixels.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float PointsToPixels(float pt) {
            if (pt < 0) {
                throw new ArgumentOutOfRangeException(nameof(pt), pt, $"Invalid font size: {pt} points.");
            }

            return pt * 4 / 3;
        }

    }
}
