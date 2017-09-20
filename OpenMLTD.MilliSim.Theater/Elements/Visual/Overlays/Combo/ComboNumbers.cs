using System;
using System.Drawing;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D.Advanced;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Theater.Elements.Visual.Gaming;
using OpenMLTD.MilliSim.Theater.Extensions;

namespace OpenMLTD.MilliSim.Theater.Elements.Visual.Overlays.Combo {
    public class ComboNumbers : Element2D {

        public ComboNumbers(GameBase game)
            : base(game) {
        }

        public uint Value { get; set; }

        protected override void OnDraw(GameTime gameTime, RenderContext context) {
            base.OnDraw(gameTime, context);

            var settings = Program.Settings;

            var gamingArea = Game.AsTheaterDays().FindSingleElement<GamingArea>();
            if (gamingArea == null) {
                throw new InvalidOperationException();
            }
            
            var location = Location;
            var images = _numberImages;
            var sourceBlankEdge = settings.Images.Combo.Numbers.UnitBlankEdge;
            var singleImageSize = gamingArea.ScaleResults.ComboNumber;

            var scaleX = singleImageSize.Width / images.UnitWidth;
            var scaleY = singleImageSize.Height / images.UnitHeight;
            var actualImageSize = new SizeF(singleImageSize.Width - (sourceBlankEdge.Left + sourceBlankEdge.Right) * scaleX, singleImageSize.Height - (sourceBlankEdge.Top + sourceBlankEdge.Bottom) * scaleY);

            context.Begin2D();

            var i = 1;
            var value = Value;
            do {
                var lower = (int)(value % 10);

                var x = location.X - i * actualImageSize.Width;
                var y = location.Y - actualImageSize.Height;

                context.DrawImageStripUnit(images, lower, x, y, actualImageSize.Width, actualImageSize.Height, sourceBlankEdge.Left, sourceBlankEdge.Top, sourceBlankEdge.Right, sourceBlankEdge.Bottom);

                value /= 10;
                ++i;
            } while (value > 0);

            context.End2D();
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);

            var settings = Program.Settings;

            var numbersSettings = settings.Images.Combo.Numbers;

            _numberImages = Direct2DHelper.LoadImageStrip(context, numbersSettings.File, numbersSettings.Count, numbersSettings.Orientation);

            var clientSize = context.ClientSize;
            var layout = settings.UI.Combo.Numbers.Layout;

            var x = layout.X.IsPercentage ? layout.X.Value * clientSize.Width : layout.X.Value;
            var y = layout.Y.IsPercentage ? layout.Y.Value * clientSize.Height : layout.Y.Value;

            Location = new Point((int)x, (int)y);
        }

        protected override void OnLostContext(RenderContext context) {
            _numberImages.Dispose();

            base.OnLostContext(context);
        }

        private D2DImageStrip _numberImages;

    }
}
