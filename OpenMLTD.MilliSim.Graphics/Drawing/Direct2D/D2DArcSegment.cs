using System.Drawing;
using OpenMLTD.MilliSim.Graphics.Extensions;
using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Graphics.Drawing.Direct2D {
    public struct D2DArcSegment {

        public D2DArcSize ArcSize { get; set; }

        public PointF Point { get; set; }

        public float RotationAngle { get; set; }

        public SizeF Size { get; set; }

        public SweepDirection SweepDirection { get; set; }

        internal ArcSegment ToNative() {
            return new ArcSegment {
                ArcSize = (ArcSize)ArcSize,
                Point = Point.ToD2DVector(),
                RotationAngle = RotationAngle,
                Size = Size.ToD2DSizeF(),
                SweepDirection = (SharpDX.Direct2D1.SweepDirection)SweepDirection
            };
        }

    }
}
