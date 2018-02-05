using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    public static class Texture2DExtensions {

        public static Bitmap SaveToBitmap([NotNull] this Texture2D texture) {
            var width = texture.Width;
            var height = texture.Height;
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            var rect = new Rectangle(0, 0, width, height);
            var textureData = new int[width * height];

            texture.GetData(textureData);

            var bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            var safePtr = bitmapData.Scan0;

            Marshal.Copy(textureData, 0, safePtr, textureData.Length);

            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

    }
}
