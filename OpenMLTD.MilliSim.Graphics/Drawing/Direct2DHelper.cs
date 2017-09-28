using System.Drawing.Imaging;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D.Advanced;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;

namespace OpenMLTD.MilliSim.Graphics.Drawing {
    public static class Direct2DHelper {

        public static D2DImageStrip LoadImageStrip(RenderContext context, string fileName, int count, ImageStripOrientation orientation) {
            var bitmap = LoadBitmap(context.RenderTarget.DeviceContext2D, fileName);
            return new D2DImageStrip(bitmap, count, orientation);
        }

        public static D2DImageStrip2D LoadImageStrip2D(RenderContext context, string fileName, float unitWidth, float unitHeight, int count, int arrayCount, ImageStripOrientation orientation) {
            var bitmap = LoadBitmap(context.RenderTarget.DeviceContext2D, fileName);
            return new D2DImageStrip2D(bitmap, unitWidth, unitHeight, count, arrayCount, orientation);
        }

        public static D2DBitmap LoadBitmap(RenderContext context, string fileName) {
            var bitmap = LoadBitmap(context.RenderTarget.DeviceContext2D, fileName);
            return new D2DBitmap(bitmap);
        }

        public static D2DImageStrip LoadImageStrip(RenderContext context, System.Drawing.Bitmap bitmap, int count, ImageStripOrientation orientation) {
            var bmp = LoadBitmap(context.RenderTarget.DeviceContext2D, bitmap);
            return new D2DImageStrip(bmp, count, orientation);
        }

        public static D2DImageStrip2D LoadImageStrip2D(RenderContext context, System.Drawing.Bitmap bitmap, float unitWidth, float unitHeight, int count, int arrayCount, ImageStripOrientation orientation) {
            var bmp = LoadBitmap(context.RenderTarget.DeviceContext2D, bitmap);
            return new D2DImageStrip2D(bmp, unitWidth, unitHeight, count, arrayCount, orientation);
        }

        public static D2DBitmap LoadBitmap(RenderContext context, System.Drawing.Bitmap bitmap) {
            var bmp = LoadBitmap(context.RenderTarget.DeviceContext2D, bitmap);
            return new D2DBitmap(bmp);
        }

        private static Bitmap LoadBitmap(SharpDX.Direct2D1.RenderTarget target, System.Drawing.Bitmap bitmap) {
            var format = new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied);
            var bmpProps = new BitmapProperties(format);
            var bmp = new Bitmap(target, new Size2(bitmap.Width, bitmap.Height), bmpProps);

            var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            bmp.CopyFromMemory(bitmapData.Scan0, bitmapData.Stride);
            bitmap.UnlockBits(bitmapData);

            return bmp;
        }

        private static Bitmap LoadBitmap(SharpDX.Direct2D1.RenderTarget target, string fileName) {
            using (var bmp = WicHelper.LoadBitmapSourceFromFile(fileName)) {
                return Bitmap.FromWicBitmap(target, bmp);
            }
        }

    }
}
