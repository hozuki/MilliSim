using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Graphics.Drawing.Direct2D.Advanced {
    public sealed class D2DImageStrip2D : D2DBitmap {

        public D2DImageStrip2D(Bitmap bitmap, float unitWidth, float unitHeight, int count, int arrayCount, ImageStripOrientation orientation)
            : base(bitmap) {
            Count = count;
            Orientation = orientation;
            UnitWidth = unitWidth;
            UnitHeight = unitHeight;
            ArrayCount = arrayCount;
        }

        public ImageStripOrientation Orientation { get; }

        public int Count { get; }

        public float UnitWidth { get; }

        public float UnitHeight { get; }

        public int ArrayCount { get; }

    }
}
