using System.Diagnostics;
using System.Drawing;
using SharpDX.Mathematics.Interop;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    public static class ColorExtensions {

        [DebuggerStepThrough]
        public static RawColor4 ToRC4(this Color color) {
            var a = (float)color.A / byte.MaxValue;
            var r = (float)color.R / byte.MaxValue;
            var g = (float)color.G / byte.MaxValue;
            var b = (float)color.B / byte.MaxValue;
            return new RawColor4(r, g, b, a);
        }

        [DebuggerStepThrough]
        public static RawColor3 ToRC3(this Color color) {
            var r = (float)color.R / byte.MaxValue;
            var g = (float)color.G / byte.MaxValue;
            var b = (float)color.B / byte.MaxValue;
            return new RawColor3(r, g, b);
        }

        [DebuggerStepThrough]
        public static RawColorBGRA ToBgra(this Color color) {
            return new RawColorBGRA(color.B, color.G, color.R, color.A);
        }

    }
}
