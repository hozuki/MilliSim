using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Advanced;
using OpenMLTD.MilliSim.Graphics.Extensions;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Overlays.Combo {
    public class ComboNumbers : Visual, IVisual2D {

        public ComboNumbers([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        public uint Value { get; set; }

        public Vector2 Location { get; set; }

        protected override void OnDraw(GameTime gameTime) {
            base.OnDraw(gameTime);

            var config = ConfigurationStore.Get<ComboNumbersConfig>();
            var game = Game.ToBaseGame();

            var scalingResponder = game.FindSingleElement<MltdStageScalingResponder>();
            if (scalingResponder == null) {
                throw new InvalidOperationException();
            }

            var location = Location;
            var images = _numberImages;
            var sourceBlankEdge = config.Data.Images.UnitBlankEdge;
            var singleImageSize = scalingResponder.ScaleResults.ComboNumber;

            var scaleX = singleImageSize.X / images.UnitWidth;
            var scaleY = singleImageSize.Y / images.UnitHeight;
            var actualImageSize = new Vector2(singleImageSize.X - (sourceBlankEdge.Left + sourceBlankEdge.Right) * scaleX, singleImageSize.Y - (sourceBlankEdge.Top + sourceBlankEdge.Bottom) * scaleY);

            var spriteBatch = game.SpriteBatch;

            spriteBatch.BeginOnBufferedVisual();

            var i = 1;
            var value = Value;
            do {
                var digit = (int)(value % 10);

                var x = location.X - i * actualImageSize.X;
                var y = location.Y - actualImageSize.Y;

                var destRect = RectHelper.RoundToRectangle(x, y, actualImageSize.X, actualImageSize.Y);

                spriteBatch.Draw(_numberImages, digit, destRect, sourceBlankEdge.ToRectangle());

                value /= 10;
                ++i;
            } while (value > 0);

            spriteBatch.End();
        }

        protected override void OnLoadContents() {
            base.OnLoadContents();

            var graphics = Game.GraphicsDevice;

            var config = ConfigurationStore.Get<ComboNumbersConfig>();

            var numbersSettings = config.Data.Images;

            _numberImages = ContentHelper.LoadSpriteSheet1D(graphics, numbersSettings.File, numbersSettings.Count, (SpriteSheetOrientation)numbersSettings.Orientation);

            var clientSize = graphics.Viewport;
            var layout = config.Data.Layout;

            var x = layout.X.IsPercentage ? layout.X.Value * clientSize.Width : layout.X.Value;
            var y = layout.Y.IsPercentage ? layout.Y.Value * clientSize.Height : layout.Y.Value;

            Location = new Vector2(x, y);
        }

        protected override void OnUnloadContents() {
            _numberImages?.Dispose();
            _numberImages = null;

            base.OnUnloadContents();
        }

        private SpriteSheet1D _numberImages;

    }
}
