using System;
using JetBrains.Annotations;
using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Rendering {
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
