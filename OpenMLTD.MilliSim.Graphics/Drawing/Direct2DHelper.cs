using System.Drawing;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D.Advanced;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace OpenMLTD.MilliSim.Graphics.Drawing {
    public static class Direct2DHelper {

        public static D2DImageStrip LoadImageStrip(RenderContext context, string fileName, int count, ImageStripOrientation orientation) {
            var bitmap = LoadBitmap(context.RenderTarget.DeviceContext2D, fileName);
            return new D2DImageStrip(bitmap, count, orientation);
        }

        public static D2DBitmap LoadBitmap(RenderContext context, string fileName) {
            return LoadBitmap(context.RenderTarget, fileName);
        }

        public static D2DBitmap LoadBitmap(RenderTarget target, string fileName) {
            var bitmap = LoadBitmap(target.DeviceContext2D, fileName);
            return new D2DBitmap(bitmap);
        }

        public static Bitmap LoadBitmap(SharpDX.Direct2D1.RenderTarget target, string fileName) {
            using (var bmp = WicHelper.LoadBitmapSourceFromFile(fileName)) {
                return Bitmap.FromWicBitmap(target, bmp);
            }
        }

        public static Color GetFloatColor(float r, float g, float b) => GetFloatColor(1f, r, g, b);

        public static Color GetFloatColor(float a, float r, float g, float b) {
            a = a.Clamp(0, 1);
            r = r.Clamp(0, 1);
            g = g.Clamp(0, 1);
            b = b.Clamp(0, 1);
            const int m = byte.MaxValue;
            return Color.FromArgb((int)(m * a), (int)(m * r), (int)(m * g), (int)(m * b));
        }

        public static Color Lerp(Color color1, Color color2, float f) {
            f = f.Clamp(0, 1);
            int a1 = color1.A, a2 = color2.A, r1 = color1.R, r2 = color2.R, g1 = color1.G, g2 = color2.G, b1 = color1.B, b2 = color2.B;
            var mf = 1 - f;
            var a = (int)(mf * a1 + f * a2);
            var r = (int)(mf * r1 + f * r2);
            var g = (int)(mf * g1 + f * g2);
            var b = (int)(mf * b1 + f * b2);
            return Color.FromArgb(a, r, g, b);
        }

        public static float Lerp(float f1, float f2, float perc) {
            perc = perc.Clamp(0, 1);
            return (1 - perc) * f1 + perc * f2;
        }

    }
}
