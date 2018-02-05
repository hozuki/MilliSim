using System;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Contributed.Scores.Animation {
    public static class RibbonMathHelper {

        public static Vector2 CubicBezier(RibbonParameters rp, float t) {
            if (rp.IsLine) {
                throw new ArgumentException("You cannot calculate cubic Bezier curves for a linear ribbon.", nameof(rp));
            }
            var pt = MathHelperEx.CubicBezier(rp.X1, rp.Y1, rp.ControlX1, rp.ControlY1, rp.ControlX2, rp.ControlY2, rp.X2, rp.Y2, t);
            return new Vector2(pt.X, pt.Y);
        }

    }
}
