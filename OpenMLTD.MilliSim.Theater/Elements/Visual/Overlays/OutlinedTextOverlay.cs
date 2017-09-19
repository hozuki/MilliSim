using System;
using System.Drawing;
using System.IO;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D.Advanced;
using OpenMLTD.MilliSim.Graphics.Extensions;
using SharpDX.DirectWrite;
using FontStyle = OpenMLTD.MilliSim.Graphics.Drawing.FontStyle;

namespace OpenMLTD.MilliSim.Theater.Elements.Visual.Overlays {
    /// <summary>
    /// A text overlay with an extra outline.
    /// The outline makes the text easier to read on complex backgrounds, but it also requires more resources to render.
    /// Suggested for static and short texts.
    /// </summary>
    public class OutlinedTextOverlay : TextOverlayBase {

        public OutlinedTextOverlay(GameBase game)
            : base(game) {
        }

        public virtual Color StrokeColor { get; set; } = Color.Black;

        public virtual float StrokeWidth { get; set; } = 2;

        protected override void OnTextChanged(EventArgs e) {
            base.OnTextChanged(e);
            _fontPath?.Dispose();
            _fontPath = null;
        }

        protected override void OnDraw(GameTime gameTime, RenderContext context) {
            base.OnDraw(gameTime, context);

            var text = Text;

            if (!string.IsNullOrWhiteSpace(text)) {
                if (_fontPath == null) {
                    RegenerateFontPath(context);
                }

                var fontPath = _fontPath;

                OnBeforeTextRendering(context, fontPath.Size, fontPath.LineHeight);

                context.Begin2D();

                var location = Location;
                context.DrawPath(_textStroke, fontPath, location.X, location.Y);
                context.FillPath(_textFill, fontPath, location.X, location.Y);

                context.End2D();
            }
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);
            var fontPath = Path.GetFullPath(Program.Settings.Window.Fonts.UI);
            var familyName = DirectWriteHelper.GetFontFamilyName(fontPath);
            _font = new D2DFont(context.DirectWriteFactory, familyName, FontSize, FontStyle.Regular, 10);
            _textFill = new D2DSolidBrush(context, FillColor);
            _textStroke = new D2DPen(context, StrokeColor, StrokeWidth);
            _fontFile = new FontFile(context.DirectWriteFactory, fontPath);
            _fontFace = new FontFace(context.DirectWriteFactory, FontFaceType.Unknown, new[] { _fontFile }, 0, FontSimulations.None);
            _glyphRun = new GlyphRun {
                FontFace = _fontFace,
                FontSize = FontSize
            };
            RegenerateFontPath(context);
        }

        protected override void OnLostContext(RenderContext context) {
            base.OnLostContext(context);
            _font.Dispose();
            _glyphRun.Dispose();
            _fontFace.Dispose();
            _fontFile.Dispose();
            _textFill.Dispose();
            _textStroke.Dispose();
            _fontPath?.Dispose();
        }

        private void RegenerateFontPath(RenderContext context) {
            var text = Text;
            if (text == null) {
                return;
            }
            _fontPath = new D2DFontPathData(text, context, _font, _fontFace);
        }

        private D2DFontPathData _fontPath;

        private FontFile _fontFile;
        private FontFace _fontFace;
        private GlyphRun _glyphRun;

        private ID2DBrush _textFill;
        private ID2DPen _textStroke;
        private D2DFont _font;

    }
}
