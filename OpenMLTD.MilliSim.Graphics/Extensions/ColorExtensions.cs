using System.Diagnostics;
using SharpDX;
using SharpDX.Mathematics.Interop;
using Color = System.Drawing.Color;

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

        [DebuggerStepThrough]
        public static Color4 ToC4(this Color color) {
            var a = (float)color.A / byte.MaxValue;
            var r = (float)color.R / byte.MaxValue;
            var g = (float)color.G / byte.MaxValue;
            var b = (float)color.B / byte.MaxValue;
            return new Color4(r, g, b, a);
        }

        public static Color ToGdiColor(this Color4 color) {
            var a = (int)(color.Alpha * byte.MaxValue);
            var r = (int)(color.Red * byte.MaxValue);
            var g = (int)(color.Green * byte.MaxValue);
            var b = (int)(color.Blue * byte.MaxValue);
            return Color.FromArgb(a, r, g, b);
        }

        public static Color ToGdiColor(this RawColor4 color) {
            var a = (int)(color.A * byte.MaxValue);
            var r = (int)(color.R * byte.MaxValue);
            var g = (int)(color.G * byte.MaxValue);
            var b = (int)(color.B * byte.MaxValue);
            return Color.FromArgb(a, r, g, b);
        }

        public static Color ToGdiColor(this Color3 color) {
            var r = (int)(color.Red * byte.MaxValue);
            var g = (int)(color.Green * byte.MaxValue);
            var b = (int)(color.Blue * byte.MaxValue);
            return Color.FromArgb(r, g, b);
        }

        public static Color ToGdiColor(this RawColor3 color) {
            var r = (int)(color.R * byte.MaxValue);
            var g = (int)(color.G * byte.MaxValue);
            var b = (int)(color.B * byte.MaxValue);
            return Color.FromArgb(r, g, b);
        }

    }
}
