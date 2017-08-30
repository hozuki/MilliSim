using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Rendering.Drawing {
    internal sealed class NativeBrushWrapper : D2DBrush, ID2DBrush {

        // Should only expose to D2DPen.
        internal NativeBrushWrapper(Brush nativeBrush)
            : this(nativeBrush, true) {
        }

        internal NativeBrushWrapper(Brush nativeBrush, bool autoDispose) {
            NativeBrush = nativeBrush;
            _autoDispose = autoDispose;
        }

        internal Brush NativeBrush { get; }

        Brush ID2DBrush.NativeBrush => NativeBrush;

        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (_autoDispose) {
                    NativeBrush?.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private readonly bool _autoDispose;

    }
}
