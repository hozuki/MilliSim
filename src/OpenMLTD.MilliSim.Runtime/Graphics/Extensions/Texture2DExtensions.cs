using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    /// <summary>
    /// Helper functions for <see cref="Texture2D"/>.
    /// </summary>
    public static class Texture2DExtensions {

        /// <summary>
        /// Saves the content of a <see cref="Texture2D"/> to a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="texture">The <see cref="Texture2D"/> to save.</param>
        /// <returns>A <see cref="Bitmap"/> containing the image data of original <see cref="Texture2D"/>.</returns>
        /// <exception cref="ArgumentException">The texture to save is not in the Color (ARGB8888) format.</exception>
        [NotNull]
        public static Bitmap SaveToBitmap([NotNull] this Texture2D texture) {
            if (texture.Format != SurfaceFormat.Color) {
                throw new ArgumentException("The texture to save is not in the Color (ARGB8888) format.");
            }

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
