using System.Drawing;
using SharpDX.Mathematics.Interop;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    public static class PointExtensions {

        public static RawVector2 ToD2DVector(this Point point) {
            return new RawVector2(point.X, point.Y);
        }

        public static RawPoint ToD2DPoint(this Point point) {
            return new RawPoint(point.X, point.Y);
        }

    }
}
