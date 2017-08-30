using System;
using OpenMLTD.MilliSim.Rendering.Drawing;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace OpenMLTD.MilliSim.Rendering.Extensions {
    public static class Direct2DDeviceContextExtensions {

        public static void DrawImage(this DeviceContext context, ID2DImage image, float destX, float destY) {
            context.DrawImage(image, destX, destY, InterpolationMode.Linear, CompositeMode.SourceOver);
        }

        public static void DrawImage(this DeviceContext context, ID2DImage image, float destX, float destY, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            var dstOffset = new RawVector2(destX, destY);
            context.DrawImage(image.NativeImage, dstOffset, null, interpolationMode, compositeMode);
        }

        public static void DrawImage(this DeviceContext context, ID2DImage image, float destX, float destY, float srcX, float srcY, float srcWidth, float srcHeight) {
            context.DrawImage(image, destX, destY, srcX, srcY, srcWidth, srcHeight, InterpolationMode.Linear, CompositeMode.SourceOver);
        }

        public static void DrawImage(this DeviceContext context, ID2DImage image, float destX, float destY, float srcX, float srcY, float srcWidth, float srcHeight, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            var dstOffset = new RawVector2(destX, destY);
            var srcRect = new RawRectangleF(srcX, srcY, srcX + srcWidth, srcY + srcHeight);
            context.DrawImage(image.NativeImage, dstOffset, srcRect, interpolationMode, compositeMode);
        }

        public static void DrawImage(this DeviceContext context, D2DEffect effect) {
            context.DrawImage(effect.NativeEffect);
        }

        public static void DrawImage(this DeviceContext context, D2DEffect effect, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            context.DrawImage(effect.NativeEffect, interpolationMode, compositeMode);
        }

        public static void DrawImage(this DeviceContext context, D2DEffect effect, float destX, float destY) {
            var dstOffset = new RawVector2(destX, destY);
            context.DrawImage(effect.NativeEffect, dstOffset);
        }

        public static void DrawImage(this DeviceContext context, D2DEffect effect, float destX, float destY, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            var dstOffset = new RawVector2(destX, destY);
            context.DrawImage(effect.NativeEffect, dstOffset, interpolationMode, compositeMode);
        }

    }
}
