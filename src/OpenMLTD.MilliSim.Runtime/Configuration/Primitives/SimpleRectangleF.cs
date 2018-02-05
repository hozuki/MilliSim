using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Configuration.Primitives {
    public struct SimpleRectangleF {

        public float Left { get; set; }

        public float Top { get; set; }

        public float Right { get; set; }

        public float Bottom { get; set; }

        public Rectangle ToRectangle() {
            var width = Right - Left;
            var height = Bottom - Top;

            return new Rectangle(MathHelperEx.Round(Left), MathHelperEx.Round(Top), MathHelperEx.Round(width), MathHelperEx.Round(height));
        }

    }
}
