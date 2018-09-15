using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Graphics {
    /// <summary>
    /// Helper functions for <see cref="Rectangle"/>.
    /// </summary>
    public static class RectHelper {

        /// <summary>
        /// Rounds a floating point dimension values to a <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Rounded <see cref="Rectangle"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle RoundToRectangle(float x, float y, float width, float height) {
            return new Rectangle(MathHelperEx.Round(x), MathHelperEx.Round(y), MathHelperEx.Round(width), MathHelperEx.Round(height));
        }

    }
}
