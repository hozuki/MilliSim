using OpenMLTD.MilliSim.Core;
using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Rendering.Drawing {
    public class D2DImage : DisposableBase {

        public D2DImage(Image image) {
            NativeImage = image;
        }

        public Image NativeImage { get; }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                NativeImage.Dispose();
            }
        }

    }
}
