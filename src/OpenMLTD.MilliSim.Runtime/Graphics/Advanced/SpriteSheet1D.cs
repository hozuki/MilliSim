using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Graphics.Advanced {
    /// <inheritdoc />
    /// <summary>
    /// A sprite sheet tiles on only one dimension.
    /// </summary>
    public sealed class SpriteSheet1D : DisposableBase {

        /// <summary>
        /// Creates a new <see cref="SpriteSheet1D"/> instance.
        /// </summary>
        /// <param name="texture">The <see cref="Texture2D"/> to create from.</param>
        /// <param name="count">Number of units.</param>
        /// <param name="orientation">Orientation of this <see cref="SpriteSheet1D"/>.</param>
        private SpriteSheet1D([NotNull] Texture2D texture, int count, SpriteSheetOrientation orientation) {
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

        /// <summary>
        /// Creates a new <see cref="SpriteSheet1D"/> from <see cref="Texture2D"/> and other parameters.
        /// </summary>
        /// <param name="texture">The <see cref="Texture2D"/> to create from.</param>
        /// <param name="count">Number of units.</param>
        /// <param name="orientation">Orientation of this <see cref="SpriteSheet1D"/>.</param>
        /// <returns>Created <see cref="SpriteSheet1D"/>.</returns>
        [NotNull]
        public static SpriteSheet1D Wrap([NotNull] Texture2D texture, int count, SpriteSheetOrientation orientation) {
            return new SpriteSheet1D(texture, count, orientation);
        }

        /// <summary>
        /// Treats current <see cref="SpriteSheet1D"/> as a <see cref="Texture2D"/> in accessing.
        /// </summary>
        /// <param name="spriteSheet">The <see cref="SpriteSheet1D"/> object.</param>
        [NotNull]
        public static implicit operator Texture2D([NotNull] SpriteSheet1D spriteSheet) {
            return spriteSheet._texture;
        }

        /// <summary>
        /// Gets the orientation of the <see cref="SpriteSheet1D"/>.
        /// </summary>
        public SpriteSheetOrientation Orientation { get; }

        /// <summary>
        /// Gets the number of units in the <see cref="SpriteSheet1D"/>.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets the width of each unit.
        /// </summary>
        public float UnitWidth { get; }

        /// <summary>
        /// Gets the height of each unit.
        /// </summary>
        public float UnitHeight { get; }

        /// <summary>
        /// Gets the underlying <see cref="Texture2D"/>.
        /// </summary>
        internal Texture2D Texture => _texture;

        protected override void Dispose(bool disposing) {
            _texture?.Dispose();
            _texture = null;
        }

        private Texture2D _texture;

    }
}
