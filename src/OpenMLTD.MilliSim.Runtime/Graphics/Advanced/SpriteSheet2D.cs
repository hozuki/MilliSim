using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Graphics.Advanced {
    public sealed class SpriteSheet2D : DisposableBase {

        private SpriteSheet2D(Texture2D texture, float unitWidth, float unitHeight, int stride, int arrayCount, SpriteSheetOrientation orientation) {
            Stride = stride;
            Orientation = orientation;
            UnitWidth = unitWidth;
            UnitHeight = unitHeight;
            ArrayCount = arrayCount;
            _texture = texture;
        }

        [NotNull]
        public static SpriteSheet2D Wrap(Texture2D texture, float unitWidth, float unitHeight, int stride, int arrayCount, SpriteSheetOrientation orientation) {
            return new SpriteSheet2D(texture, unitWidth, unitHeight, stride, arrayCount, orientation);
        }

        [NotNull]
        public static implicit operator Texture2D(SpriteSheet2D spriteSheet) {
            return spriteSheet._texture;
        }

        public SpriteSheetOrientation Orientation { get; }

        public int Stride { get; }

        public float UnitWidth { get; }

        public float UnitHeight { get; }

        public int ArrayCount { get; }

        internal Texture2D Texture => _texture;

        protected override void Dispose(bool disposing) {
            _texture?.Dispose();
            _texture = null;
        }

        private Texture2D _texture;

    }
}
