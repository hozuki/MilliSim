using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Graphics.Drawing {
    public interface IBufferedElement2D {

        void OnCopyBufferedContents(RenderContext context, Bitmap buffer);

    }
}
