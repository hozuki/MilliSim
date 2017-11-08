using System.IO;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D;
using OpenMLTD.MilliSim.Graphics.Extensions;
using FontStyle = OpenMLTD.MilliSim.Graphics.Drawing.FontStyle;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    /// <summary>
    /// A basic text overlay.
    /// Suggested for dynamic texts or long texts.
    /// </summary>
    public class TextOverlay : TextOverlayBase {

        public TextOverlay([NotNull] IVisualContainer parent)
            : base(parent) {
        }

        protected override void OnDraw(GameTime gameTime, RenderContext context) {
            base.OnDraw(gameTime, context);

            var text = Text;
            if (!string.IsNullOrWhiteSpace(text)) {
                var textSize = DirectWriteHelper.MeasureText(context.DirectWriteFactory, text, _font);
                var lineHeight = DirectWriteHelper.PointToDip(FontSize);
                OnBeforeTextRendering(context, textSize, lineHeight);

                var location = Location;
                context.Begin2D();
                context.DrawText(text, _textBrush, _font, location.X, location.Y);
                context.End2D();
            }
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);
            var fontPath = Path.GetFullPath(GetFontFilePath());
            var familyName = DirectWriteHelper.GetFontFamilyName(fontPath);
            _font = new D2DFont(context.DirectWriteFactory, familyName, FontSize, FontStyle.Regular, 10);

            _textBrush = new D2DSolidBrush(context, FillColor);
        }

        protected override void OnLostContext(RenderContext context) {
            base.OnLostContext(context);
            _textBrush.Dispose();
            _font.Dispose();
        }

        private ID2DBrush _textBrush;
        private D2DFont _font;

    }
}
