using System;

namespace OpenMLTD.MilliSim.Rendering {
    /// <inheritdoc cref="IDisposable"/>
    /// <summary>
    /// Used to swich <see cref="RenderTarget"/>s when using a <see cref="RenderContext"/>.
    /// </summary>
    public struct TargetSwitcher : IDisposable {

        public static TargetSwitcher Begin(RenderContext context, RenderTarget usingTarget) {
            var t = new TargetSwitcher {
                _context = context,
                _originalTarget = context.RenderTarget
            };
            context.SetRenderTarget(usingTarget);
            return t;
        }

        void IDisposable.Dispose() {
            _context.SetRenderTarget(_originalTarget);
        }

        private RenderContext _context;
        private RenderTarget _originalTarget;

    }
}
