using System;
using System.Drawing;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.GameAbstraction.Extensions;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D.Advanced;
using OpenMLTD.MilliSim.Graphics.Extensions;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Overlays.Combo {
    public class ComboNumbers : Visual2D {

        public ComboNumbers([NotNull] IVisualContainer parent)
            : base(parent) {
        }

        public uint Value { get; set; }

        protected override void OnDraw(GameTime gameTime, RenderContext context) {
            base.OnDraw(gameTime, context);

            var config = ConfigurationStore.Get<ComboNumbersConfig>();

            var scalingResponder = Game.AsTheaterDays().FindSingleElement<MltdStageScalingResponder>();
            if (scalingResponder == null) {
                throw new InvalidOperationException();
            }

            var location = Location;
            var images = _numberImages;
            var sourceBlankEdge = config.Data.Images.UnitBlankEdge;
            var singleImageSize = scalingResponder.ScaleResults.ComboNumber;

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

            var config = ConfigurationStore.Get<ComboNumbersConfig>();

            var numbersSettings = config.Data.Images;

            _numberImages = Direct2DHelper.LoadImageStrip(context, numbersSettings.File, numbersSettings.Count, (ImageStripOrientation)numbersSettings.Orientation);

            var clientSize = context.ClientSize;
            var layout = config.Data.Layout;

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
