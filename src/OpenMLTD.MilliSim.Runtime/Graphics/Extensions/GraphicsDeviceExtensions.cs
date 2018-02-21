using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    public static class GraphicsDeviceExtensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDisposable SwitchTo([NotNull] this GraphicsDevice graphicsDevice, [NotNull] RenderTarget2D renderTarget) {
            var target = new RenderTargetSwither(graphicsDevice, renderTarget);

            graphicsDevice.Clear(Color.Transparent);

            return target;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        private struct RenderTargetSwither : IDisposable {

            internal RenderTargetSwither([NotNull] GraphicsDevice graphicsDevice, [NotNull] RenderTarget2D renderTarget) {
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
