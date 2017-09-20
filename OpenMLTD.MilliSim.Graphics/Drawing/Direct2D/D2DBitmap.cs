using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Graphics.Drawing.Direct2D {
    public class D2DBitmap : D2DImage, ID2DImage {

        public D2DBitmap(Bitmap bitmap)
            : base(bitmap) {
            NativeBitmap = bitmap;
        }

        public float Width => NativeBitmap.Size.Width;

        public float Height => NativeBitmap.Size.Height;

        public D2DBitmap ShareWith(RenderContext context) {
            return ShareWith(context.RenderTarget.DeviceContext2D);
        }

        public D2DBitmap ShareWith(DeviceContext context) {
            var bitmap = new Bitmap(context, NativeBitmap);
            return new D2DBitmap(bitmap);
        }

        internal Bitmap NativeBitmap { get; }

        Image ID2DImage.NativeImage => NativeBitmap;

    }
}
