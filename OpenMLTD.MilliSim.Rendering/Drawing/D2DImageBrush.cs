using System.Drawing;
using OpenMLTD.MilliSim.Rendering.Extensions;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using Brush = SharpDX.Direct2D1.Brush;

namespace OpenMLTD.MilliSim.Rendering.Drawing {
    public sealed class D2DImageBrush : D2DBrushBase, ID2DBrush {

        public D2DImageBrush(RenderContext context, D2DBitmap bitmap, ExtendMode extendModeX, ExtendMode extendModeY, InterpolationMode interpolationMode) {
            var brushProperties = new ImageBrushProperties {
                ExtendModeX = extendModeX,
                ExtendModeY = extendModeY,
                InterpolationMode = interpolationMode,
                SourceRectangle = new RawRectangleF(0, 0, bitmap.Width, bitmap.Height)
            };
            NativeBrush = new ImageBrush(context.Direct2DDeviceContext, bitmap.NativeImage, brushProperties);
        }

        public D2DImageBrush(RenderContext context, ID2DImage image, ExtendMode extendModeX, ExtendMode extendModeY, InterpolationMode interpolationMode, RectangleF sourceRectangle) {
            var brushProperties = new ImageBrushProperties {
                ExtendModeX = extendModeX,
                ExtendModeY = extendModeY,
                InterpolationMode = interpolationMode,
                SourceRectangle = sourceRectangle.ToD2DRectF()
            };
            NativeBrush = new ImageBrush(context.Direct2DDeviceContext, image.NativeImage, brushProperties);
        }

        public D2DImageBrush(RenderContext context, ID2DImage image, ImageBrushProperties brushProperties) {
            NativeBrush = new ImageBrush(context.Direct2DDeviceContext, image.NativeImage, brushProperties);
        }

        internal ImageBrush NativeBrush { get; }

        Brush ID2DBrush.NativeBrush => NativeBrush;

        protected override void Dispose(bool disposing) {
            if (disposing) {
                NativeBrush.Dispose();
            }
        }

    }
}
