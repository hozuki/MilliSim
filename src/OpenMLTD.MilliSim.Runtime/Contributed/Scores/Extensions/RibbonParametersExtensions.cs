using System.Drawing;
using OpenMLTD.MilliSim.Contributed.Scores.Animation;

namespace OpenMLTD.MilliSim.Contributed.Scores.Extensions {
    public static class RibbonParametersExtensions {

        public static void Deconstruct(this RibbonParameters rp, out float x1, out float y1, out float cx1, out float cy1, out float cx2, out float cy2, out float x2, out float y2, out bool isLine) {
            x1 = rp.X1;
            y1 = rp.Y1;
            cx1 = rp.ControlX1;
            cy1 = rp.ControlY1;
            cx2 = rp.ControlX2;
            cy2 = rp.ControlY2;
            x2 = rp.X2;
            y2 = rp.Y2;
            isLine = rp.IsLine;
        }

        public static PointF CalculateCubicBezier(this RibbonParameters rp, float t) {
            return RibbonMathHelper.CubicBezier(rp, t);
        }

    }
}
