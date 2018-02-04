using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Graphics {
    public static class RectHelper {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle RoundToRectangle(float x, float y, float width, float height) {
            return new Rectangle(MathHelperEx.Round(x), MathHelperEx.Round(y), MathHelperEx.Round(width), MathHelperEx.Round(height));
        }

    }
}
