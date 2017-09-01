using System.Drawing;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Graphics.Extensions;
using SharpDX.DirectWrite;

namespace OpenMLTD.MilliSim.Graphics.Drawing.Direct2D.Advanced {
    // https://www.codeproject.com/Articles/376597/Outline-Text-With-DirectWrite
    public sealed class D2DFontPathData : DisposableBase {

        public D2DFontPathData([NotNull] string text, RenderContext context, D2DFont d2dFont, FontFace fontFace) {
            Initialize(context, text, d2dFont, fontFace);
            Size = context.MeasureText(text, d2dFont);
        }

        public D2DFontPathData([NotNull] string text, RenderContext context, D2DFont d2dFont, FontFace fontFace, float maxWidth, float maxHeight) {
            Initialize(context, text, d2dFont, fontFace);
            Size = context.MeasureText(text, d2dFont, maxWidth, maxHeight);
        }

        public string Text { get; private set; }

        public D2DFont D2DFont { get; private set; }

        public FontFace FontFace { get; private set; }

        public SizeF Size { get; }

        public float LineHeight { get; private set; }

        internal void Draw(RenderContext context, ID2DPen pen) {
            const float offsetX = 0;
            var offsetY = 0f;
            var pathDataArray = _pathDataArray;
            var lineHeights = _lineHeights;
            var len = pathDataArray.Length;
            for (var i = 0; i < len; ++i) {
                var pathData = pathDataArray[i];
                if (pathData != null) {
                    context.DrawPath(pen, pathData, offsetX, offsetY);
                }
                offsetY += lineHeights[i];
            }
        }

        internal void Draw(RenderContext context, ID2DPen pen, float offsetX, float offsetY) {
            context.PushTransform();
            context.Translate(offsetX, offsetY);
            Draw(context, pen);
            context.PopTransform();
        }

        internal void Fill(RenderContext context, ID2DBrush brush) {
            const float offsetX = 0;
            var offsetY = 0f;
            var pathDataArray = _pathDataArray;
            var lineHeights = _lineHeights;
            var len = pathDataArray.Length;
            for (var i = 0; i < len; ++i) {
                var pathData = pathDataArray[i];
                if (pathData != null) {
                    context.FillPath(brush, pathData, offsetX, offsetY);
                }
                offsetY += lineHeights[i];
            }
        }

        internal void Fill(RenderContext context, ID2DBrush brush, float offsetX, float offsetY) {
            context.PushTransform();
            context.Translate(offsetX, offsetY);
            Fill(context, brush);
            context.PopTransform();
        }

        protected override bool KeepFinalizer { get; } = true;

        protected override void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }

            foreach (var pathData in _pathDataArray) {
                pathData?.Dispose();
            }
        }

        private void Initialize(RenderContext context, string text, D2DFont d2dFont, FontFace fontFace) {
            Text = text;
            D2DFont = d2dFont;
            FontFace = fontFace;

            var fontSizeInEm = DirectWriteHelper.PointToDip(d2dFont.Size);
            LineHeight = fontSizeInEm;

            if (text.IndexOf('\n') >= 0) {
                var processedText = text.Replace("\r", string.Empty);
                var lines = processedText.Split('\n');

                _pathDataArray = new D2DPathData[lines.Length];
                _lineHeights = new float[lines.Length];

                for (var i = 0; i < lines.Length; ++i) {
                    var line = lines[i];
                    if (!string.IsNullOrWhiteSpace(line)) {
                        var pathData = GetPathData(context, line, fontFace, fontSizeInEm);
                        _pathDataArray[i] = pathData;
                    }
                    _lineHeights[i] = fontSizeInEm;
                }
            } else {
                var pathData = GetPathData(context, text, fontFace, fontSizeInEm);
                _pathDataArray = new[] { pathData };
                _lineHeights = new[] { fontSizeInEm };
            }
        }

        // Warning: this function seems to have serious memory leak...
        private static D2DPathData GetPathData(RenderContext context, string text, FontFace fontFace, float fontSizeInEm) {
            var pathData = new D2DPathData(context.RenderTarget.DeviceContext.Factory);

            pathData.BeginDraw();

            var codePoints = new int[text.Length];
            for (var i = 0; i < text.Length; ++i) {
                codePoints[i] = char.ConvertToUtf32(text, i);
            }

            var indices = fontFace.GetGlyphIndices(codePoints);
            fontFace.GetGlyphRunOutline(fontSizeInEm, indices, null, null, false, false, pathData.NativeSink);

            pathData.EndDraw();

            return pathData;
        }

        private D2DPathData[] _pathDataArray;
        private float[] _lineHeights;

    }
}
