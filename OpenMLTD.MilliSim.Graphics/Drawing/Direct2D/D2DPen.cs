using System.Drawing;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Graphics.Extensions;
using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Graphics.Drawing.Direct2D {
    public sealed class D2DPen : DisposableBase, ID2DPen {

        public D2DPen(RenderContext context, Color color)
            : this(context, color, 1) {
        }

        public D2DPen(RenderContext context, Color color, float strokeWidth) {
            var nativeBrush = new SolidColorBrush(context.RenderTarget.DeviceContext, color.ToRC4());
            Brush = new NativeBrushWrapper(nativeBrush);
            StrokeStyle = null;
            StrokeWidth = strokeWidth;
            _isExternalBrush = false;
        }

        public D2DPen(ID2DBrush brush)
            : this(brush, 1) {
        }

        public D2DPen(ID2DBrush brush, float strokeWidth)
            : this(brush, strokeWidth, null) {
        }

        public D2DPen(ID2DBrush brush, float strokeWidth, StrokeStyle style) {
            Brush = brush;
            StrokeWidth = strokeWidth;
            StrokeStyle = style;
            _isExternalBrush = true;
        }

        public StrokeStyle StrokeStyle { get; }

        public ID2DBrush Brush { get; }

        public float StrokeWidth { get; }

        protected override void Dispose(bool disposing) {
            if (disposing && !_isExternalBrush) {
                Brush?.Dispose();
            }
        }

        private readonly bool _isExternalBrush;

    }
}
