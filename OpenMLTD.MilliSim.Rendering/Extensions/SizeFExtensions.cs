using System.Drawing;
using SharpDX;

namespace OpenMLTD.MilliSim.Rendering.Extensions {
    public static class SizeFExtensions {

        public static Size2F ToD2DSize(this SizeF size) {
            return new Size2F(size.Width, size.Height);
        }

        public static SizeF ToGdiSize(this Size2F size) {
            return new SizeF(size.Width, size.Height);
        }

    }
}
