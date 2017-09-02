using System;
using JetBrains.Annotations;
using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Graphics {
    public static class TargetSwitcher {

        public static IDisposable Begin2D(RenderContext context, [NotNull] RenderTarget usingTarget) {
            return new TargetSwitcher2D(context, usingTarget);
        }

        /// <summary>
        /// Directly switches the underlying <see cref="SharpDX.Direct2D1.RenderTarget"/>'s target.
        /// Please note that this switching is only available to 2D drawing.
        /// </summary>
        /// <param name="context">The <see cref="RenderContext"/> to use.</param>
        /// <param name="buffer">The temporary target.</param>
        /// <returns>An <see cref="IDisposable"/>. Calling <see cref="IDisposable.Dispose"/> will reset <see cref="context"/>'s target.</returns>
        public static IDisposable Begin2D(RenderContext context, [NotNull] Image buffer) {
            return new TargetSwitcher2D(context, buffer);
        }

        /// <inheritdoc cref="IDisposable"/>
        /// <summary>
        /// Used to swich <see cref="RenderTarget"/>s when using a <see cref="RenderContext"/>.
        /// </summary>
        private struct TargetSwitcher2D : IDisposable {

            internal TargetSwitcher2D(RenderContext context, [NotNull] RenderTarget usingTarget) {
                _context = context;
                _originalTarget = context.RenderTarget;
                _originalTargetImage = null;
                _isWrapped = true;

                context.SetRenderTarget(usingTarget);
            }

            internal TargetSwitcher2D(RenderContext context, [NotNull] Image buffer) {
                _context = context;
                _originalTarget = null;
                _originalTargetImage = context.RenderTarget.DeviceContext2D.Target;
                _isWrapped = false;

                context.RenderTarget.DeviceContext2D.Target = buffer;
            }

            void IDisposable.Dispose() {
                if (_isWrapped) {
                    _context.SetRenderTarget(_originalTarget);
                } else {
                    _context.RenderTarget.DeviceContext2D.Target = _originalTargetImage;
                }
            }

            private readonly RenderContext _context;
            private readonly RenderTarget _originalTarget;
            private readonly Image _originalTargetImage;

            private readonly bool _isWrapped;

        }

    }
}
