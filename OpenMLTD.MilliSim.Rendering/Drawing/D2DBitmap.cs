using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Rendering.Drawing {
    public sealed class D2DBitmap : D2DImageBase, ID2DImage {

        public D2DBitmap(Bitmap bitmap) {
            NativeImage = bitmap;
        }

        public Bitmap NativeImage { get; }

        public float Width => NativeImage.Size.Width;

        public float Height => NativeImage.Size.Height;

        Image ID2DImage.NativeImage => NativeImage;

        protected override void Dispose(bool disposing) {
            if (disposing) {
                NativeImage.Dispose();
            }
        }

    }
}
