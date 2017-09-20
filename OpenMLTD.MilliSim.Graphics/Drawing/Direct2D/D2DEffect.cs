using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Graphics.Drawing.Direct2D {
    public abstract class D2DEffect : D2DImage, ID2DImage {

        protected D2DEffect(Effect effect)
            : base(effect.Output) {
            NativeEffect = effect;
        }

        public Effect NativeEffect { get; }

        public void SetInput(int index, ID2DImage input) {
            SetInput(index, input, true);
        }

        public void SetInput(int index, ID2DImage input, bool invalidate) {
            NativeEffect.SetInput(index, input.NativeImage, invalidate);
        }

        public void SetInput(int index, D2DEffect input) {
            NativeEffect.SetInputEffect(index, input.NativeEffect);
        }

        public void SetInput(int index, D2DEffect input, bool invalidate) {
            NativeEffect.SetInputEffect(index, input.NativeEffect, invalidate);
        }

        Image ID2DImage.NativeImage => NativeEffect.Output;

        protected override void Dispose(bool disposing) {
            if (disposing) {
                NativeEffect.Dispose();
            }
        }

    }
}
