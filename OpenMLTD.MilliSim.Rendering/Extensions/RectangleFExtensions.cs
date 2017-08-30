using System.Drawing;
using SharpDX.Mathematics.Interop;

namespace OpenMLTD.MilliSim.Rendering.Extensions {
    public static class RectangleFExtensions {

        public static RawRectangleF ToD2DRectF(this RectangleF rect) {
            return new RawRectangleF(rect.X, rect.Y, rect.Right, rect.Bottom);
        }

    }
}
