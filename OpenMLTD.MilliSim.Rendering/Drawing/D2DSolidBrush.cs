using System.Drawing;
using OpenMLTD.MilliSim.Rendering.Extensions;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace OpenMLTD.MilliSim.Rendering.Drawing {
    public sealed class D2DSolidBrush : D2DBrush, ID2DBrush {

        public D2DSolidBrush(RenderContext context, Color color) {
            NativeBrush = new SolidColorBrush(context.RenderTarget.Direct2DRenderTarget, color.ToRC4());
        }

        public D2DSolidBrush(RenderContext context, RawColor4 color) {
            NativeBrush = new SolidColorBrush(context.RenderTarget.Direct2DRenderTarget, color);
        }

        public D2DSolidBrush(SharpDX.Direct2D1.RenderTarget target, Color color) {
            NativeBrush = new SolidColorBrush(target, color.ToRC4());
        }

        public D2DSolidBrush(SharpDX.Direct2D1.RenderTarget target, RawColor4 color) {
            NativeBrush = new SolidColorBrush(target, color);
        }

        internal SolidColorBrush NativeBrush { get; }

        public Color Color {
            get => NativeBrush.Color.ToColor();
            set => NativeBrush.Color = value.ToRC4();
        }

        SharpDX.Direct2D1.Brush ID2DBrush.NativeBrush => NativeBrush;

        protected override void Dispose(bool disposing) {
            if (disposing) {
                NativeBrush.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
