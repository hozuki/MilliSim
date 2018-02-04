using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Graphics.Advanced;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    public static class SpriteBatchExtensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Draw([NotNull] this SpriteBatch spriteBatch, [NotNull] Texture2D texture, Rectangle destRect, float opacity = 1.0f) {
            spriteBatch.Draw(texture, destRect, Color.White * opacity);
        }

        public static void Draw([NotNull] this SpriteBatch spriteBatch, SpriteSheet1D spriteSheet, int index, Rectangle destRect, float opacity = 1.0f) {
            if (index < 0 || index >= spriteSheet.Count) {
                return;
            }

            if (opacity <= 0) {
                return;
            }

            var srcRect = GetSourceRect(spriteSheet, index);

            spriteBatch.Draw(spriteSheet.Texture, destRect, srcRect, Color.White * opacity);
        }

        public static void Draw([NotNull] this SpriteBatch spriteBatch, SpriteSheet1D spriteSheet, int index, Rectangle destRect, Rectangle blankEdge) {
            if (index < 0 || index >= spriteSheet.Count) {
                return;
            }

            var srcRect = GetSourceRect(spriteSheet, index, blankEdge);

            spriteBatch.Draw(spriteSheet.Texture, destRect, srcRect, Color.White);
        }

        public static void Draw([NotNull] this SpriteBatch spriteBatch, SpriteSheet1D spriteSheet, int index, float x, float y) {
            if (index < 0 || index >= spriteSheet.Count) {
                return;
            }

            var srcRect = GetSourceRect(spriteSheet, index);

            spriteBatch.Draw(spriteSheet.Texture, new Vector2(x, y), srcRect, Color.White);
        }

        public static void Draw([NotNull] this SpriteBatch spriteBatch, SpriteSheet2D spriteSheet, int index, float x, float y) {
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
        /// Used for copying <see cref="IBufferedVisual"/> contents only
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
                    xIndex = index % spriteSheet.ArrayCount;
                    yIndex = index / spriteSheet.ArrayCount;
                    break;
                case SpriteSheetOrientation.Vertical:
                    w = spriteSheet.UnitWidth;
                    h = spriteSheet.UnitHeight;
                    xIndex = index / spriteSheet.ArrayCount;
                    yIndex = index % spriteSheet.ArrayCount;
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
