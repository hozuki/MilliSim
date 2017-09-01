using System.Drawing;
using SharpDX.Mathematics.Interop;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    public static class RawColorExtensions {

        public static Color ToColor(this RawColor3 color) {
            var r = (int)(color.R * 255);
            var g = (int)(color.G * 255);
            var b = (int)(color.B * 255);
            return Color.FromArgb(r, g, b);
        }

        public static Color ToColor(this RawColor4 color) {
            var a = (int)(color.A * 255);
            var r = (int)(color.R * 255);
            var g = (int)(color.G * 255);
            var b = (int)(color.B * 255);
            return Color.FromArgb(a, r, g, b);
        }

    }
}
