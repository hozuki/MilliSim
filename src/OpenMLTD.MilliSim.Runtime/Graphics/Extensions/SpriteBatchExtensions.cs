using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Graphics.Advanced;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    /// <summary>
    /// Extended functions for <see cref="SpriteBatch"/>.
    /// </summary>
    public static class SpriteBatchExtensions {

        /// <summary>
        /// Draws a <see cref="Texture2D"/>, scaled to fit a destination rectangle, with specified opacity.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> to use.</param>
        /// <param name="texture">The <see cref="Texture2D"/> to draw.</param>
        /// <param name="destRect">Destination rectangle.</param>
        /// <param name="opacity">Opacity.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Draw([NotNull] this SpriteBatch spriteBatch, [NotNull] Texture2D texture, Rectangle destRect, float opacity = 1.0f) {
            opacity = MathHelper.Clamp(opacity, 0, 1);
            spriteBatch.Draw(texture, destRect, Color.White * opacity);
        }

        /// <summary>
        /// Draws a sprite of <see cref="SpriteSheet1D"/> to fit a destination rectangle, with specified opacity.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> to use.</param>
        /// <param name="spriteSheet">The <see cref="SpriteSheet1D"/> to use.</param>
        /// <param name="index">The index of the sprite in the <see cref="SpriteSheet1D"/>.</param>
        /// <param name="destRect">Destination rectangle.</param>
        /// <param name="opacity">Opacity.</param>
        public static void Draw([NotNull] this SpriteBatch spriteBatch, [NotNull] SpriteSheet1D spriteSheet, int index, Rectangle destRect, float opacity = 1.0f) {
            if (index < 0 || index >= spriteSheet.Count) {
                return;
            }

            if (opacity <= 0) {
                return;
            }

            var srcRect = GetSourceRect(spriteSheet, index);

            opacity = MathHelper.Clamp(opacity, 0, 1);

            spriteBatch.Draw(spriteSheet.Texture, destRect, srcRect, Color.White * opacity);
        }

        /// <summary>
        /// Draws a sprite of <see cref="SpriteSheet1D"/> to fit a destination rectangle.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> to use.</param>
        /// <param name="spriteSheet">The <see cref="SpriteSheet1D"/> to use.</param>
        /// <param name="index">The index of the sprite in the <see cref="SpriteSheet1D"/>.</param>
        /// <param name="destRect">Destination rectangle.</param>
        /// <param name="blankEdge">Blank edge (margin) settings.</param>
        public static void Draw([NotNull] this SpriteBatch spriteBatch, [NotNull] SpriteSheet1D spriteSheet, int index, Rectangle destRect, Rectangle blankEdge) {
            if (index < 0 || index >= spriteSheet.Count) {
                return;
            }

            var srcRect = GetSourceRect(spriteSheet, index, blankEdge);

            spriteBatch.Draw(spriteSheet.Texture, destRect, srcRect, Color.White);
        }

        /// <summary>
        /// Draws a sprite of <see cref="SpriteSheet1D"/> unscaled to a specified location.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> to use.</param>
        /// <param name="spriteSheet">The <see cref="SpriteSheet1D"/> to use.</param>
        /// <param name="index">The index of the sprite in the <see cref="SpriteSheet1D"/>.</param>
        /// <param name="x">X coordinate of the location, in pixels.</param>
        /// <param name="y">Y coordinate of the location, in pixels.</param>
        public static void Draw([NotNull] this SpriteBatch spriteBatch, [NotNull] SpriteSheet1D spriteSheet, int index, float x, float y) {
            if (index < 0 || index >= spriteSheet.Count) {
                return;
            }

            var srcRect = GetSourceRect(spriteSheet, index);

            spriteBatch.Draw(spriteSheet.Texture, new Vector2(x, y), srcRect, Color.White);
        }

        /// <summary>
        /// Draws a sprite of <see cref="SpriteSheet2D"/> unscaled to a specified location.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> to use.</param>
        /// <param name="spriteSheet">The <see cref="SpriteSheet2D"/> to use.</param>
        /// <param name="index">The index of the sprite in the <see cref="SpriteSheet2D"/>.</param>
        /// <param name="x">X coordinate of the location, in pixels.</param>
        /// <param name="y">Y coordinate of the location, in pixels.</param>
        public static void Draw([NotNull] this SpriteBatch spriteBatch, [NotNull] SpriteSheet2D spriteSheet, int index, float x, float y) {
            if (index < 0 || index >= spriteSheet.ArrayCount) {
                return;
            }

            var srcRect = GetSourceRect(spriteSheet, index);

            spriteBatch.Draw(spriteSheet.Texture, new Vector2(x, y), srcRect, Color.White);
        }

        /// <summary>
        /// Used for direct draw calls for objects in <see cref="IBufferedVisual"/>s or in children of <see cref="IBufferedVisual"/>s.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> to use.</param>
        public static void BeginOnBufferedVisual([NotNull] this SpriteBatch spriteBatch) {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
        }

        /// <summary>
        /// Draws the contents on the <see cref="IBufferedVisual"/> onto current render target. Used for copying <see cref="IBufferedVisual"/> contents only.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> to use.</param>
        /// <param name="visual">The <see cref="IBufferedVisual"/> to copy.</param>
        public static void DrawBufferedVisual([NotNull] this SpriteBatch spriteBatch, [NotNull] IBufferedVisual visual) {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, transformMatrix: visual.Transform);
            spriteBatch.Draw(visual.BufferTarget, visual.Location, Color.White * visual.Opacity);
            spriteBatch.End();
        }

        private static Rectangle GetSourceRect([NotNull] SpriteSheet1D spriteSheet, int index, Rectangle blankEdge) {
            var rect = GetSourceRect(spriteSheet, index);
            return new Rectangle(rect.Left + blankEdge.Left, rect.Top + blankEdge.Top, rect.Width - (blankEdge.Left + blankEdge.Right), rect.Height - (blankEdge.Top + blankEdge.Bottom));
        }

        private static Rectangle GetSourceRect([NotNull] SpriteSheet1D spriteSheet, int index) {
            if (index < 0 || index >= spriteSheet.Count) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            float x, y, w, h;
            switch (spriteSheet.Orientation) {
                case SpriteSheetOrientation.Horizontal:
                    w = spriteSheet.UnitWidth;
                    h = spriteSheet.UnitHeight;
                    x = index * w;
                    y = 0;
                    break;
                case SpriteSheetOrientation.Vertical:
                    w = spriteSheet.UnitWidth;
                    h = spriteSheet.UnitHeight;
                    x = 0;
                    y = index * h;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return RectHelper.RoundToRectangle(x, y, w, h);
        }

        private static Rectangle GetSourceRect([NotNull] SpriteSheet2D spriteSheet, int index) {
            if (index < 0 || index >= spriteSheet.ArrayCount) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            int xIndex, yIndex;
            float w, h;
            switch (spriteSheet.Orientation) {
                case SpriteSheetOrientation.Horizontal:
                    w = spriteSheet.UnitWidth;
                    h = spriteSheet.UnitHeight;
                    xIndex = index % spriteSheet.Stride;
                    yIndex = index / spriteSheet.Stride;
                    break;
                case SpriteSheetOrientation.Vertical:
                    w = spriteSheet.UnitWidth;
                    h = spriteSheet.UnitHeight;
                    xIndex = index / spriteSheet.Stride;
                    yIndex = index % spriteSheet.Stride;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var x = xIndex * w;
            var y = yIndex * h;

            return RectHelper.RoundToRectangle(x, y, w, h);
        }

    }
}
