using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Graphics.Advanced;

namespace OpenMLTD.MilliSim.Foundation {
    /// <summary>
    /// Game content helper functions.
    /// </summary>
    public static class ContentHelper {

        /// <summary>
        /// Loads a <see cref="Texture2D"/> from file.
        /// </summary>
        /// <param name="graphics">The <see cref="GraphicsDevice"/> for this <see cref="Texture2D"/>.</param>
        /// <param name="path">Path to the file.</param>
        /// <returns>Loaded <see cref="Texture2D"/>.</returns>
        [NotNull]
        public static Texture2D LoadTexture([NotNull] GraphicsDevice graphics, [NotNull] string path) {
            using (var fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                return Texture2D.FromStream(graphics, fileStream);
            }
        }

        /// <summary>
        /// Loads a <see cref="Texture2D"/> from <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="graphics">The <see cref="GraphicsDevice"/> for this <see cref="Texture2D"/>.</param>
        /// <param name="bitmap">The <see cref="Bitmap"/> containing image data.</param>
        /// <param name="format">Image format.</param>
        /// <returns>Loaded <see cref="Texture2D"/>.</returns>
        public static Texture2D LoadTexture([NotNull] GraphicsDevice graphics, [NotNull] Bitmap bitmap, [NotNull] ImageFormat format) {
            using (var memoryStream = new MemoryStream()) {
                bitmap.Save(memoryStream, format);
                memoryStream.Position = 0;

                return Texture2D.FromStream(graphics, memoryStream);
            }
        }

        /// <summary>
        /// Loads a <see cref="Texture2D"/> from file and wraps it to a <see cref="SpriteSheet1D"/>.
        /// </summary>
        /// <param name="graphics">The <see cref="GraphicsDevice"/> for this <see cref="Texture2D"/>.</param>
        /// <param name="filePath">Path to the file.</param>
        /// <param name="count">Number of sprites in the <see cref="SpriteSheet1D"/>.</param>
        /// <param name="orientation">The orientation of the <see cref="SpriteSheet1D"/>.</param>
        /// <returns>Loaded <see cref="SpriteSheet1D"/>.</returns>
        [NotNull]
        public static SpriteSheet1D LoadSpriteSheet1D([NotNull] GraphicsDevice graphics, [NotNull] string filePath, int count, SpriteSheetOrientation orientation) {
            var texture = LoadTexture(graphics, filePath);

            return SpriteSheet1D.Wrap(texture, count, orientation);
        }

        /// <summary>
        /// Loads a <see cref="Texture2D"/> from a <see cref="Bitmap"/> and wraps it to a <see cref="SpriteSheet1D"/>.
        /// </summary>
        /// <param name="graphics">The <see cref="GraphicsDevice"/> for this <see cref="Texture2D"/>.</param>
        /// <param name="bitmap">The <see cref="Bitmap"/> containing image data.</param>
        /// <param name="format">Image format.</param>
        /// <param name="count">Number of sprites in the <see cref="SpriteSheet1D"/>.</param>
        /// <param name="orientation">The orientation of the <see cref="SpriteSheet1D"/>.</param>
        /// <returns>Loaded <see cref="SpriteSheet1D"/>.</returns>
        [NotNull]
        public static SpriteSheet1D LoadSpriteSheet1D([NotNull] GraphicsDevice graphics, [NotNull] Bitmap bitmap, [NotNull] ImageFormat format, int count, SpriteSheetOrientation orientation) {
            var texture = LoadTexture(graphics, bitmap, format);

            return SpriteSheet1D.Wrap(texture, count, orientation);
        }

        /// <summary>
        /// Loads a <see cref="Texture2D"/> from a <see cref="Bitmap"/> and wraps it to a <see cref="SpriteSheet2D"/>.
        /// </summary>
        /// <param name="graphics">The <see cref="GraphicsDevice"/> for this <see cref="Texture2D"/>.</param>
        /// <param name="bitmap">The <see cref="Bitmap"/> containing image data.</param>
        /// <param name="format">Image format.</param>
        /// <param name="unitWidth">Width of each unit, in pixels.</param>
        /// <param name="unitHeight">Height of each unit, in pixels.</param>
        /// <param name="stride">Number of units in a column (orientation is horizontal) or row (orientation is vertical).</param>
        /// <param name="arrayCount">Total units in the <see cref="SpriteSheet2D"/>.</param>
        /// <param name="orientation">Orientation of the <see cref="SpriteSheet2D"/>.</param>
        /// <returns>Loaded <see cref="SpriteSheet2D"/>.</returns>
        [NotNull]
        public static SpriteSheet2D LoadSpriteSheet2D([NotNull] GraphicsDevice graphics, [NotNull] Bitmap bitmap, [NotNull] ImageFormat format, float unitWidth, float unitHeight, int stride, int arrayCount, SpriteSheetOrientation orientation) {
            var texture = LoadTexture(graphics, bitmap, format);

            return SpriteSheet2D.Wrap(texture, unitWidth, unitHeight, stride, arrayCount, orientation);
        }

    }
}
