using OpenMLTD.MilliSim.Core;
using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Graphics.Drawing.Direct2D {
    public class D2DImage : DisposableBase, ID2DImage {

        internal D2DImage(Image image) {
            NativeImage = image;
        }

        internal Image NativeImage { get; }

        Image ID2DImage.NativeImage => NativeImage;

        protected override void Dispose(bool disposing) {
            if (disposing) {
                NativeImage.Dispose();
            }
        }

    }
}
