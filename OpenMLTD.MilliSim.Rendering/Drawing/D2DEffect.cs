using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Rendering.Drawing {
    public class D2DEffect : D2DImageBase, ID2DImage {

        public D2DEffect(Effect effect) {
            NativeEffect = effect;
        }

        public Effect NativeEffect { get; }

        Image ID2DImage.NativeImage => NativeEffect.Output;

        protected override void Dispose(bool disposing) {
            if (disposing) {
                NativeEffect.Dispose();
            }
        }

    }
}
