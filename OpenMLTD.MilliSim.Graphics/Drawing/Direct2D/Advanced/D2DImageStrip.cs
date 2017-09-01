using System;
using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Graphics.Drawing.Direct2D.Advanced {
    public sealed class D2DImageStrip : D2DBitmap {

        public D2DImageStrip(Bitmap bitmap, int count, ImageStripOrientation orientation)
            : base(bitmap) {
            Count = count;
            Orientation = orientation;

            var size = bitmap.PixelSize;
            switch (orientation) {
                case ImageStripOrientation.Horizontal:
                    UnitWidth = (float)size.Width / count;
                    UnitHeight = size.Height;
                    break;
                case ImageStripOrientation.Vertical:
                    UnitWidth = size.Width;
                    UnitHeight = (float)size.Height / count;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }

        public ImageStripOrientation Orientation { get; }

        public int Count { get; }

        public float UnitWidth { get; }

        public float UnitHeight { get; }

    }
}
