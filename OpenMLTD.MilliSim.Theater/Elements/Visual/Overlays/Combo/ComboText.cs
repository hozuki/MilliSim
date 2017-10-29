using System;
using System.Drawing;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Theater.Elements.Visual.Gaming;
using OpenMLTD.MilliSim.Theater.Extensions;

namespace OpenMLTD.MilliSim.Theater.Elements.Visual.Overlays.Combo {
    public class ComboText : Visual2D {

        public ComboText([NotNull] IVisualContainer parent)
            : base(parent) {
        }

        protected override void OnDraw(GameTime gameTime, RenderContext context) {
            base.OnDraw(gameTime, context);

            var theaterDays = Game.AsTheaterDays();

            var gamingArea = theaterDays.FindSingleElement<GamingArea>();
            if (gamingArea == null) {
                throw new InvalidOperationException();
            }

            var scaledSize = gamingArea.ScaleResults.ComboText;
            var location = Location;

            context.Begin2D();
            context.DrawBitmap(_textImage, location.X, location.Y, scaledSize.Width, scaledSize.Height);
            context.End2D();
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);

            var settings = Program.Settings;

            _textImage = Direct2DHelper.LoadBitmap(context, settings.Images.Combo.Text.FileName);

            var clientSize = context.ClientSize;
            var layout = settings.UI.Combo.Text.Layout;

            var x = layout.X.ToActualValue(clientSize.Width);
            var y = layout.Y.ToActualValue(clientSize.Height);

            Location = new Point((int)x, (int)y);
        }

        protected override void OnLostContext(RenderContext context) {
            _textImage.Dispose();

            base.OnLostContext(context);
        }

        private D2DBitmap _textImage;

    }
}
