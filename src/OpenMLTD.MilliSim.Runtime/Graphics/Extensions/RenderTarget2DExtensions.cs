using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    public static class RenderTarget2DExtensions {

        public static RenderTarget2D CreateCompatibleRenderTarget([NotNull] this RenderTarget2D renderTarget) {
            return new RenderTarget2D(renderTarget.GraphicsDevice, renderTarget.Width, renderTarget.Height, false, renderTarget.Format, renderTarget.DepthStencilFormat, 0, RenderTargetUsage.PreserveContents);
        }

    }
}
