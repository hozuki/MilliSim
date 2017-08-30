using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Rendering {
    public interface IBufferedElement {

        void OnCopyBufferedContents(RenderContext context, Bitmap buffer);

    }
}
