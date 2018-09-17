using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Graphics.Advanced {
    /// <inheritdoc />
    /// <summary>
    /// A sprite sheet that tiles on two dimensions.
    /// </summary>
    public sealed class SpriteSheet2D : DisposableBase {

        /// <summary>
        /// Creates a new <see cref="SpriteSheet2D"/> instance.
        /// </summary>
        /// <param name="texture">The <see cref="Texture2D"/> to create from.</param>
        /// <param name="unitWidth">Width of each unit.</param>
        /// <param name="unitHeight">Height of each unit.</param>
        /// <param name="stride">Number of units per column (when orientation is <see cref="SpriteSheetOrientation.Horizontal"/>) or row (when orientation is <see cref="SpriteSheetOrientation.Vertical"/>).</param>
        /// <param name="arrayCount">The total number of units in this <see cref="SpriteSheet2D"/>.</param>
        /// <param name="orientation">Orientation of this <see cref="SpriteSheet2D"/>.</param>
        private SpriteSheet2D([NotNull] Texture2D texture, float unitWidth, float unitHeight, int stride, int arrayCount, SpriteSheetOrientation orientation) {
            Stride = stride;
            Orientation = orientation;
            UnitWidth = unitWidth;
            UnitHeight = unitHeight;
            ArrayCount = arrayCount;
            _texture = texture;
        }

        /// <summary>
        /// Creates a new <see cref="SpriteSheet2D"/> from <see cref="Texture2D"/> and other parameters.
        /// </summary>
        /// <param name="texture">The <see cref="Texture2D"/> to create from.</param>
        /// <param name="unitWidth">Width of each unit.</param>
        /// <param name="unitHeight">Height of each unit.</param>
        /// <param name="stride">Number of units per column (when orientation is <see cref="SpriteSheetOrientation.Horizontal"/>) or row (when orientation is <see cref="SpriteSheetOrientation.Vertical"/>).</param>
        /// <param name="arrayCount">The total number of units in this <see cref="SpriteSheet2D"/>.</param>
        /// <param name="orientation">Orientation of this <see cref="SpriteSheet2D"/>.</param>
        /// <returns>Created <see cref="SpriteSheet2D"/>.</returns>
        [NotNull]
        public static SpriteSheet2D Wrap([NotNull] Texture2D texture, float unitWidth, float unitHeight, int stride, int arrayCount, SpriteSheetOrientation orientation) {
            return new SpriteSheet2D(texture, unitWidth, unitHeight, stride, arrayCount, orientation);
        }

        /// <summary>
        /// Treats current <see cref="SpriteSheet2D"/> as a <see cref="Texture2D"/> in accessing.
        /// </summary>
        /// <param name="spriteSheet">The <see cref="SpriteSheet2D"/> object.</param>
        [NotNull]
        public static implicit operator Texture2D(SpriteSheet2D spriteSheet) {
            return spriteSheet._texture;
        }

        /// <summary>
        /// Gets the orientation of the <see cref="SpriteSheet1D"/>.
        /// </summary>
        public SpriteSheetOrientation Orientation { get; }

        /// <summary>
        /// Gets the number of units per column (when <see cref="Orientation"/> is <see cref="SpriteSheetOrientation.Horizontal"/>) or row (when <see cref="Orientation"/> is <see cref="SpriteSheetOrientation.Vertical"/>).
        /// </summary>
        public int Stride { get; }

        /// <summary>
        /// Gets the width of each unit.
        /// </summary>
        public float UnitWidth { get; }

        /// <summary>
        /// Gets the height of each unit.
        /// </summary>
        public float UnitHeight { get; }

        /// <summary>
        /// Gets the total number of units in this <see cref="SpriteSheet2D"/>.
        /// </summary>
        public int ArrayCount { get; }

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
