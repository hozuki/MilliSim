using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace OpenMLTD.MilliSim.Rendering.Extensions {
    partial class RenderContextExtensions {

        public static void DrawImage(this RenderContext context, Image image, float destX, float destY) {
            context.DrawImage(image, destX, destY, InterpolationMode.Linear, CompositeMode.SourceOver);
        }

        public static void DrawImage(this RenderContext context, Image image, float destX, float destY, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            var dstOffset = new RawVector2(destX, destY);
            context.Direct2DDeviceContext.DrawImage(image, dstOffset, null, interpolationMode, compositeMode);
        }

        public static void DrawImage(this RenderContext context, Image image, float destX, float destY, float srcX, float srcY, float srcWidth, float srcHeight) {
            context.DrawImage(image, destX, destY, srcX, srcY, srcWidth, srcHeight, InterpolationMode.Linear, CompositeMode.SourceOver);
        }

        public static void DrawImage(this RenderContext context, Image image, float destX, float destY, float srcX, float srcY, float srcWidth, float srcHeight, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            var dstOffset = new RawVector2(destX, destY);
            var srcRect = new RawRectangleF(srcX, srcY, srcX + srcWidth, srcY + srcHeight);
            context.Direct2DDeviceContext.DrawImage(image, dstOffset, srcRect, interpolationMode, compositeMode);
        }

    }
}
