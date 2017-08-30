using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace OpenMLTD.MilliSim.Rendering.Extensions {
    partial class RenderContextExtensions {

        public static void DrawImage(this RenderContext context, Image image, float destX, float destY) {
            context.DrawImage(image, destX, destY, InterpolationMode.Linear, CompositeMode.SourceOver);
        }

        public static void DrawImage(this RenderContext context, Image image, float destX, float destY, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            var dstOffset = new RawVector2(destX, destY);
            context.RenderTarget.DeviceContext.DrawImage(image, dstOffset, null, interpolationMode, compositeMode);
        }

        public static void DrawImage(this RenderContext context, Image image, float destX, float destY, float srcX, float srcY, float srcWidth, float srcHeight) {
            context.DrawImage(image, destX, destY, srcX, srcY, srcWidth, srcHeight, InterpolationMode.Linear, CompositeMode.SourceOver);
        }

        public static void DrawImage(this RenderContext context, Image image, float destX, float destY, float srcX, float srcY, float srcWidth, float srcHeight, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            var dstOffset = new RawVector2(destX, destY);
            var srcRect = new RawRectangleF(srcX, srcY, srcX + srcWidth, srcY + srcHeight);
            context.RenderTarget.DeviceContext.DrawImage(image, dstOffset, srcRect, interpolationMode, compositeMode);
        }

        public static void DrawBitmap(this RenderContext context, Bitmap bitmap) {
            context.DrawBitmap(bitmap, 1f);
        }

        public static void DrawBitmap(this RenderContext context, Bitmap bitmap, float opacity) {
            context.RenderTarget.DeviceContext.DrawBitmap(bitmap, opacity, BitmapInterpolationMode.Linear);
        }

        public static void DrawBitmap(this RenderContext context, Bitmap bitmap, float destX, float destY, float opacity) {
            var size = bitmap.PixelSize;
            var destRect = new RawRectangleF(destX, destY, destX + size.Width, destY + size.Height);
            context.RenderTarget.DeviceContext.DrawBitmap(bitmap, destRect, opacity, BitmapInterpolationMode.Linear);
        }

    }
}
