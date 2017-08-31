using System.Drawing;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Rendering.Drawing;
using OpenMLTD.MilliSim.Rendering.Drawing.Advanced;
using SharpDX.WIC;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using PixelFormat = SharpDX.WIC.PixelFormat;

namespace OpenMLTD.MilliSim.Rendering {
    public static class Direct2DHelper {

        public static BitmapSource LoadBitmapAsWic(string fileName) {
            using (var factory = new ImagingFactory()) {
                using (var decoder = new BitmapDecoder(factory, fileName, DecodeOptions.CacheOnDemand)) {
                    var converter = new FormatConverter(factory);
                    converter.Initialize(decoder.GetFrame(0), PixelFormat.Format32bppPBGRA, BitmapDitherType.None, null, 0, BitmapPaletteType.Custom);
                    return converter;
                }
            }
        }

        public static D2DImageStrip LoadImageStrip(string fileName, RenderContext context, int count, ImageStripOrientation orientation) {
            var bitmap = LoadBitmap(fileName, context.RenderTarget.DeviceContext);
            return new D2DImageStrip(bitmap, count, orientation);
        }

        public static D2DBitmap LoadBitmap(string fileName, RenderContext context) {
            return LoadBitmap(fileName, context.RenderTarget);
        }

        public static D2DBitmap LoadBitmap(string fileName, RenderTarget target) {
            var bitmap = LoadBitmap(fileName, target.DeviceContext);
            return new D2DBitmap(bitmap);
        }

        public static Bitmap LoadBitmap(string fileName, SharpDX.Direct2D1.RenderTarget target) {
            using (var bmp = LoadBitmapAsWic(fileName)) {
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
