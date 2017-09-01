using System.Drawing;
using SharpDX.Mathematics.Interop;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    public static class PointFExtensions {

        public static RawVector2 ToD2DVector(this PointF point) {
            return new RawVector2(point.X, point.Y);
        }

        public static PointF ToGdiPointF(this RawVector2 vector) {
            return new PointF(vector.X, vector.Y);
        }

    }
}
