using System;
using JetBrains.Annotations;
using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Graphics {
    /// <inheritdoc cref="IDisposable"/>
    /// <summary>
    /// Used to swich <see cref="RenderTarget"/>s when using a <see cref="RenderContext"/>.
    /// </summary>
    public struct TargetSwitcher : IDisposable {

        public static IDisposable Begin(RenderContext context, [NotNull] RenderTarget usingTarget) {
            var t = new TargetSwitcher {
                _context = context,
                _originalTarget = context.RenderTarget,
                _isWrapped = true
            };
            context.SetRenderTarget(usingTarget);
            return t;
        }

        /// <summary>
        /// Directly switches the underlying <see cref="SharpDX.Direct2D1.RenderTarget"/>'s target.
        /// Please note that this switching is only available to 2D drawing.
        /// </summary>
        /// <param name="context">The <see cref="RenderContext"/> to use.</param>
        /// <param name="buffer">The temporary target.</param>
        /// <returns>An <see cref="IDisposable"/>. Calling <see cref="IDisposable.Dispose"/> will reset <see cref="context"/>'s target.</returns>
        public static IDisposable Begin(RenderContext context, [NotNull] Image buffer) {
            var t = new TargetSwitcher {
                _context = context,
                _originalTargetImage = context.RenderTarget.DeviceContext.Target,
                _isWrapped = false
            };
            context.RenderTarget.DeviceContext.Target = buffer;
            return t;
        }

        void IDisposable.Dispose() {
            if (_isWrapped) {
                _context.SetRenderTarget(_originalTarget);
            } else {
                _context.RenderTarget.DeviceContext.Target = _originalTargetImage;
            }
        }

        private RenderContext _context;
        private RenderTarget _originalTarget;
        private Image _originalTargetImage;

        private bool _isWrapped;

    }
}
