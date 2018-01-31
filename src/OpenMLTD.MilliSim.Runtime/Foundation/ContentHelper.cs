using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Graphics.Advanced;

namespace OpenMLTD.MilliSim.Foundation {
    public static class ContentHelper {

        public static Texture2D LoadTexture([NotNull] GraphicsDevice graphics, [NotNull] string path) {
            using (var fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                return Texture2D.FromStream(graphics, fileStream);
            }
        }

        public static Texture2D LoadTexture([NotNull] GraphicsDevice graphics, [NotNull] Bitmap bitmap, [NotNull] ImageFormat format) {
            using (var memoryStream = new MemoryStream()) {
                bitmap.Save(memoryStream, format);
                memoryStream.Position = 0;

                return Texture2D.FromStream(graphics, memoryStream);
            }
        }

        public static SpriteSheet1D LoadSpriteSheet1D([NotNull] GraphicsDevice graphics, [NotNull] Bitmap bitmap, [NotNull] ImageFormat format, int count, SpriteSheetOrientation orientation) {
            var texture = LoadTexture(graphics, bitmap, format);

            return SpriteSheet1D.Wrap(texture, count, orientation);
        }

        public static SpriteSheet2D LoadSpriteSheet2D([NotNull] GraphicsDevice graphics, [NotNull] Bitmap bitmap, [NotNull] ImageFormat format, float unitWidth, float unitHeight, int stride, int arrayCount, SpriteSheetOrientation orientation) {
            var texture = LoadTexture(graphics, bitmap, format);

            return SpriteSheet2D.Wrap(texture, unitWidth, unitHeight, stride, arrayCount, orientation);
        }

    }
}
