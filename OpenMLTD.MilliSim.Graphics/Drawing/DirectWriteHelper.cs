using System.Drawing;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D;
using SharpDX.DirectWrite;
using SharpFont;

namespace OpenMLTD.MilliSim.Graphics.Drawing {
    public static class DirectWriteHelper {

        public static SizeF MeasureText(Factory factory, string text, D2DFont font) {
            return MeasureText(factory, text, font, float.MaxValue, float.MaxValue);
        }

        public static SizeF MeasureText(Factory factory, string text, D2DFont font, float maxWidth, float maxHeight) {
            using (var layout = new TextLayout(factory, text, font.NativeFont, maxWidth, maxHeight)) {
                var metrics = layout.Metrics;
                return new SizeF(PointToDip(metrics.Width), PointToDip(metrics.Height));
            }
        }

        public static float PointToDip(float points) {
            return (points / 72) * 96;
        }

        public static string GetFontFamilyName([NotNull] string path) {
            if (_library == null) {
                _library = new Library();
            }
            using (var face = new Face(_library, path)) {
                return face.FamilyName;
            }
        }

        // Warning: this object is not disposed.
        private static Library _library;

    }
}
