using System;
using System.Drawing;
using System.IO;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
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

        public OutlinedTextOverlay([NotNull] IVisualContainer parent)
            : base(parent) {
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
            var fontPath = Path.GetFullPath(GetFontFilePath());
            var familyName = DirectWriteHelper.GetFontFamilyName(fontPath);
            _font = new D2DFont(context.DirectWriteFactory, familyName, FontSize, FontStyle.Regular, 10);
            _textFill = new D2DSolidBrush(context, FillColor);
            _textStroke = new D2DPen(context, StrokeColor, StrokeWidth);
            _fontFile = new FontFile(context.DirectWriteFactory, fontPath);

            FontFaceType fontFaceType;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                // Remmeber to use app.manifest to make OS return the correct version.
                var ver = Environment.OSVersion.Version;
                if (ver.Major < 6) {
                    fontFaceType = FontFaceType.Unknown;
                } else if (ver.Major == 6) {
                    if (ver.Minor < 2) {
                        fontFaceType = GetFontFaceTypeFromFileName(fontPath);
                    } else {
                        // The "unknown" here means D2D can determine by itself.
                        fontFaceType = FontFaceType.Unknown;
                    }
                } else {
                    // The "unknown" here means D2D can determine by itself.
                    fontFaceType = FontFaceType.Unknown;
                }
            } else {
                fontFaceType = FontFaceType.Unknown;
            }

            _fontFace = new FontFace(context.DirectWriteFactory, fontFaceType, new[] { _fontFile }, 0, FontSimulations.None);
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

        private static FontFaceType GetFontFaceTypeFromFileName(string fileName) {
            var ext = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(ext)) {
                return FontFaceType.Unknown;
            }

            // https://fileinfo.com/filetypes/font
            switch (ext.ToLowerInvariant()) {
                case ".eot":
                case ".otf":
                    return FontFaceType.OpenTypeCollection;
                case ".ttf":
                    return FontFaceType.Truetype;
                case ".ttc":
                    return FontFaceType.TruetypeCollection;
                case ".fon":
                case ".fnt":
                // Sorry but we don't support these because we can't determine if a file is
                // bitmap font or vector font.
                default:
                    return FontFaceType.Unknown;
            }
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
