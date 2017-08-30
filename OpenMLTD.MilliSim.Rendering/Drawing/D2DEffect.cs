using OpenMLTD.MilliSim.Core;
using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Rendering.Drawing {
    public class D2DEffect : DisposableBase {

        public D2DEffect(Effect effect) {
            NativeEffect = effect;
        }

        public Effect NativeEffect { get; }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                NativeEffect.Dispose();
            }
        }

    }
}
