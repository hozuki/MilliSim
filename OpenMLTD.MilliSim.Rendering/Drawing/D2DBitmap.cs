using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Rendering.Drawing {
    public class D2DBitmap : D2DImage {

        public D2DBitmap(Bitmap bitmap)
            : base(bitmap) {
        }

        public Bitmap NativeBitmap => (Bitmap)NativeImage;

        public float Width => NativeBitmap.Size.Width;

        public float Height => NativeBitmap.Size.Height;

    }
}
