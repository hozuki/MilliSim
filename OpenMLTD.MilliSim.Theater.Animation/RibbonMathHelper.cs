using System;
using System.Drawing;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Theater.Animation {
    public static class RibbonMathHelper {

        public static PointF CubicBezier(RibbonParameters rp, float t) {
            if (rp.IsLine) {
                throw new ArgumentException("You cannot calculate cubic Bezier curves for a linear ribbon.", nameof(rp));
            }
            var pt = MathHelper.CubicBezier(rp.X1, rp.Y1, rp.ControlX1, rp.ControlY1, rp.ControlX2, rp.ControlY2, rp.X2, rp.Y2, t);
            return new PointF(pt.X, pt.Y);
        }

    }
}
