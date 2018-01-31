using System;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Graphics.Advanced {
    public sealed class SpriteSheet1D : DisposableBase {

        private SpriteSheet1D(Texture2D texture, int count, SpriteSheetOrientation orientation) {
            Count = count;
            Orientation = orientation;

            var size = new Size(texture.Width, texture.Height);
            switch (orientation) {
                case SpriteSheetOrientation.Horizontal:
                    UnitWidth = (float)size.Width / count;
                    UnitHeight = size.Height;
                    break;
                case SpriteSheetOrientation.Vertical:
                    UnitWidth = size.Width;
                    UnitHeight = (float)size.Height / count;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }

            _texture = texture;
        }

        public static SpriteSheet1D Wrap(Texture2D texture, int count, SpriteSheetOrientation orientation) {
            return new SpriteSheet1D(texture, count, orientation);
        }

        public static implicit operator Texture2D(SpriteSheet1D spriteSheet) {
            return spriteSheet._texture;
        }

        public SpriteSheetOrientation Orientation { get; }

        public int Count { get; }

        public float UnitWidth { get; }

        public float UnitHeight { get; }

        internal Texture2D Texture => _texture;

        protected override void Dispose(bool disposing) {
            _texture?.Dispose();
            _texture = null;
        }

        private Texture2D _texture;

    }
}
