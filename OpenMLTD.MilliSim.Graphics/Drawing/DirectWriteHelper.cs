using System.Drawing;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D;
using SharpDX.DirectWrite;

namespace OpenMLTD.MilliSim.Graphics.Drawing {
    public static class DirectWriteHelper {

        public static SizeF MeasureText([NotNull] Factory factory, [NotNull] string text, [NotNull] D2DFont font) {
            return MeasureText(factory, text, font, float.MaxValue, float.MaxValue);
        }

        public static SizeF MeasureText([NotNull] Factory factory, [NotNull] string text, [NotNull] D2DFont font, float maxWidth, float maxHeight) {
            using (var layout = new TextLayout(factory, text, font.NativeFont, maxWidth, maxHeight)) {
                var metrics = layout.Metrics;
                return new SizeF(PointToDip(metrics.Width), PointToDip(metrics.Height));
            }
        }

        public static float PointToDip(float points) {
            // DIP = (points / 72) * 96;
            return points / 3 * 4;
        }

        public static string GetFontFamilyName([NotNull] RenderContext context, [NotNull] string path) {
            var renderer = context.Renderer;
            var collection = renderer.PrivateFontCollection;
            var fontFamily = collection.GetFontFamilyFromFile(path);

            return fontFamily.Name;
        }

    }
}
