using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    /// <summary>
    /// Helper functions for <see cref="RenderTarget2D"/>.
    /// </summary>
    public static class RenderTarget2DExtensions {

        /// <summary>
        /// Creates a compatible <see cref="RenderTarget2D"/> from the source <see cref="RenderTarget2D"/>'s parameters.
        /// </summary>
        /// <param name="renderTarget">The source <see cref="RenderTarget2D"/>.</param>
        /// <returns>Created <see cref="RenderTarget2D"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static RenderTarget2D CreateCompatibleRenderTarget([NotNull] this RenderTarget2D renderTarget) {
            return new RenderTarget2D(renderTarget.GraphicsDevice, renderTarget.Width, renderTarget.Height, false, renderTarget.Format, renderTarget.DepthStencilFormat, 0, RenderTargetUsage.PreserveContents);
        }

    }
}
