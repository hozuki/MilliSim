using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Graphics.Advanced;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    public static class SpriteBatchExtensions {

        public static void Draw([NotNull] this SpriteBatch spriteBatch, SpriteSheet1D spriteSheet, int index, float x, float y) {
            if (index < 0 || index >= spriteSheet.Count) {
                return;
            }

            int indexX, indexY;

            switch (spriteSheet.Orientation) {
                case SpriteSheetOrientation.Horizontal:
                    indexX = index % spriteSheet.Count;
                    indexY = 0;
                    break;
                case SpriteSheetOrientation.Vertical:
                    indexX = 0;
                    indexY = index % spriteSheet.Count;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var rect = new Rectangle((int)(indexX * spriteSheet.UnitWidth), (int)(indexY * spriteSheet.UnitHeight), (int)spriteSheet.UnitWidth, (int)spriteSheet.UnitHeight);

            spriteBatch.Draw(spriteSheet.Texture, new Vector2(x, y), rect, Color.White);
        }

        public static void Draw([NotNull] this SpriteBatch spriteBatch, SpriteSheet2D spriteSheet, int index, float x, float y) {
            if (index < 0 || index >= spriteSheet.ArrayCount) {
                return;
            }

            int indexX, indexY;

            switch (spriteSheet.Orientation) {
                case SpriteSheetOrientation.Horizontal:
                    indexX = index % spriteSheet.Stride;
                    indexY = index / spriteSheet.Stride;
                    break;
                case SpriteSheetOrientation.Vertical:
                    indexX = index / spriteSheet.Stride;
                    indexY = index % spriteSheet.Stride;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var rect = new Rectangle((int)(indexX * spriteSheet.UnitWidth), (int)(indexY * spriteSheet.UnitHeight), (int)spriteSheet.UnitWidth, (int)spriteSheet.UnitHeight);

            spriteBatch.Draw(spriteSheet.Texture, new Vector2(x, y), rect, Color.White);
        }

    }
}
