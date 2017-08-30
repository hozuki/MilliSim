using System.Drawing;
using SharpDX;
using SharpDX.Mathematics.Interop;

namespace OpenMLTD.MilliSim.Rendering.Extensions {
    public static class SizeFExtensions {

        public static Size2F ToD2DSizeF(this SizeF size) {
            return new Size2F(size.Width, size.Height);
        }

        public static SizeF ToGdiSizeF(this Size2F size) {
            return new SizeF(size.Width, size.Height);
        }

        public static RawVector2 ToD2DVector(this SizeF size) {
            return new Vector2(size.Width, size.Height);
        }

        public static SizeF ToGdiSizeF(this RawVector2 size) {
            return new SizeF(size.X, size.Y);
        }

    }
}
