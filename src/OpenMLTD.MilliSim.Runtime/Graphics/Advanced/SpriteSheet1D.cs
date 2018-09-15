using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Graphics.Advanced {
    public sealed class SpriteSheet1D : DisposableBase {

        private SpriteSheet1D(Texture2D texture, int count, SpriteSheetOrientation orientation) {
            Count = count;
            Orientation = orientation;

            var size = new Vector2(texture.Width, texture.Height);
            switch (orientation) {
                case SpriteSheetOrientation.Horizontal:
                    UnitWidth = size.X / count;
                    UnitHeight = size.Y;
                    break;
                case SpriteSheetOrientation.Vertical:
                    UnitWidth = size.X;
                    UnitHeight = size.Y / count;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }

            _texture = texture;
        }

        [NotNull]
        public static SpriteSheet1D Wrap(Texture2D texture, int count, SpriteSheetOrientation orientation) {
            return new SpriteSheet1D(texture, count, orientation);
        }

        [NotNull]
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
