using System.Drawing;
using OpenMLTD.MilliSim.Rendering.Extensions;
using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Rendering.Drawing {
    public struct D2DQuadraticBezierSegment {

        public PointF Point1 { get; set; }

        public PointF Point2 { get; set; }

        internal QuadraticBezierSegment ToNative() {
            return new QuadraticBezierSegment {
                Point1 = Point1.ToD2DVector(),
                Point2 = Point2.ToD2DVector()
            };
        }

    }
}
