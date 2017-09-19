using OpenMLTD.MilliSim.Core;
using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Graphics.Drawing {
    public interface IBufferedElement2D {

        void OnCopyBufferedContents(GameTime gameTime, RenderContext context, Bitmap buffer);

    }
}
