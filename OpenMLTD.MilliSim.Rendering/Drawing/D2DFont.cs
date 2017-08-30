using OpenMLTD.MilliSim.Core;
using SharpDX.DirectWrite;

namespace OpenMLTD.MilliSim.Rendering.Drawing {
    public sealed class D2DFont : DisposableBase {

        private D2DFont(string familyName, float size, FontStyle style, float weight) {
            FamilyName = familyName;
            Size = size;
            Style = style;
            Weight = weight;
        }

        public D2DFont(Factory factory, string familyName, float size, FontStyle style, float weight)
            : this(familyName, size, style, weight) {
            Factory = factory;

            var isBold = (style & FontStyle.Bold) != 0;
            var isItalic = (style & FontStyle.Italic) != 0;

            _nativeFont = new TextFormat(factory, familyName, isBold ? FontWeight.Bold : FontWeight.Normal, isItalic ? SharpDX.DirectWrite.FontStyle.Italic : SharpDX.DirectWrite.FontStyle.Normal, size);
        }

        public string FamilyName { get; }

        public float Size { get; }

        public FontStyle Style { get; }

        public float Weight { get; }

        public Factory Factory { get; }

        public TextFormat NativeFont => _nativeFont;

        protected override void Dispose(bool disposing) {
            if (disposing) {
                _nativeFont?.Dispose();
                _nativeFont = null;
            }
        }

        private TextFormat _nativeFont;

    }
}
