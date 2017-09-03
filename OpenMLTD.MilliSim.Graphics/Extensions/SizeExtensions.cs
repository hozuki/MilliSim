using System.Drawing;
using SharpDX;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    public static class SizeExtensions {

        public static Size2F ToD2DSizeF(this Size size) {
            return new Size2F(size.Width, size.Height);
        }

        public static Size2 ToD2DSize(this Size size) {
            return new Size2(size.Width, size.Height);
        }

        public static Size ToGdiSize(this Size2 size) {
            return new Size(size.Width, size.Height);
        }

    }
}
