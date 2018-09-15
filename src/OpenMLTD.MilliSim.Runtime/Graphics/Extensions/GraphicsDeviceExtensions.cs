using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    public static class GraphicsDeviceExtensions {

        /// <summary>
        /// Switch the render target of specified <see cref="GraphicsDevice"/> to a new <see cref="RenderTarget2D"/>.
        /// </summary>
        /// <param name="graphicsDevice">The <see cref="GraphicsDevice"/>.</param>
        /// <param name="renderTarget">The <see cref="RenderTarget2D"/> to switch to.</param>
        /// <returns>An <see cref="IDisposable"/>. Call its <see cref="IDisposable.Dispose"/> method after rendering to the graphics device, to restore the original target.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static IDisposable SwitchTo([NotNull] this GraphicsDevice graphicsDevice, [NotNull] RenderTarget2D renderTarget) {
            return SwitchTo(graphicsDevice, renderTarget, true);
        }

        /// <summary>
        /// Switch the render target of specified <see cref="GraphicsDevice"/> to a new <see cref="RenderTarget2D"/>.
        /// </summary>
        /// <param name="graphicsDevice">The <see cref="GraphicsDevice"/>.</param>
        /// <param name="renderTarget">The <see cref="RenderTarget2D"/> to switch to.</param>
        /// <param name="clearTarget">Whether clears the <see cref="RenderTarget2D"/> or not after switching.</param>
        /// <returns>An <see cref="IDisposable"/>. Call its <see cref="IDisposable.Dispose"/> method after rendering to the graphics device, to restore the original target.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static IDisposable SwitchTo([NotNull] this GraphicsDevice graphicsDevice, [NotNull] RenderTarget2D renderTarget, bool clearTarget) {
            var target = new RenderTargetSwitcher(graphicsDevice, renderTarget);

            if (clearTarget) {
                graphicsDevice.Clear(Color.Transparent);
            }

            return target;
        }

        /// <summary>
        /// Creates a compatible <see cref="RenderTarget2D"/> from the <see cref="GraphicsDevice"/>'s parameters.
        /// </summary>
        /// <param name="graphicsDevice">The <see cref="GraphicsDevice"/>.</param>
        /// <returns>Created <see cref="RenderTarget2D"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static RenderTarget2D CreateCompatibleRenderTargetFromBackBuffer([NotNull] this GraphicsDevice graphicsDevice) {
            var pp = graphicsDevice.PresentationParameters;
            // TODO: Potential porting problems on smartphones and consoles (currently not our target).
            // Notice the PreserveContent parameter.
            // See:
            // 1. https://github.com/MonoGame/MonoGame/issues/1628#issuecomment-15998825
            // 2. https://gamedev.stackexchange.com/a/90407
            // 3. https://blogs.msdn.microsoft.com/shawnhar/2007/11/21/rendertarget-changes-in-xna-game-studio-2-0/
            return new RenderTarget2D(graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, pp.DepthStencilFormat, 0, RenderTargetUsage.PreserveContents);
        }

        private struct RenderTargetSwitcher : IDisposable {

            internal RenderTargetSwitcher([NotNull] GraphicsDevice graphicsDevice, [NotNull] RenderTarget2D renderTarget) {
                _graphicsDevice = graphicsDevice;
                _renderTargetBindings = graphicsDevice.GetRenderTargets();

                graphicsDevice.SetRenderTarget(renderTarget);
            }

            public void Dispose() {
                _graphicsDevice.SetRenderTargets(_renderTargetBindings);
            }

            private readonly GraphicsDevice _graphicsDevice;
            private readonly RenderTargetBinding[] _renderTargetBindings;

        }

    }
}
