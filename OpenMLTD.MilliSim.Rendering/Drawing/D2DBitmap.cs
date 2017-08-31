using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Rendering.Drawing {
    public class D2DBitmap : D2DImageBase, ID2DImage {

        public D2DBitmap(Bitmap bitmap) {
            NativeImage = bitmap;
        }

        public float Width => NativeImage.Size.Width;

        public float Height => NativeImage.Size.Height;

        public D2DBitmap ShareWith(RenderContext context) {
            return ShareWith(context.RenderTarget.DeviceContext);
        }

        public D2DBitmap ShareWith(DeviceContext context) {
            var bitmap = new Bitmap(context, NativeImage);
            return new D2DBitmap(bitmap);
        }

        internal Bitmap NativeImage { get; }

        Image ID2DImage.NativeImage => NativeImage;

        protected override void Dispose(bool disposing) {
            if (disposing) {
                NativeImage.Dispose();
            }
        }

    }
}
