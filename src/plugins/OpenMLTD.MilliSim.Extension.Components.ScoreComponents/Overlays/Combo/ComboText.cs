using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Extensions;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Overlays.Combo {
    public class ComboText : Visual, IVisual2D {

        public ComboText([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        public Vector2 Location { get; set; }

        protected override void OnDraw(GameTime gameTime) {
            base.OnDraw(gameTime);

            var theaterDays = Game.ToBaseGame();

            var scalingResponder = theaterDays.FindSingleElement<MltdStageScalingResponder>();
            if (scalingResponder == null) {
                throw new InvalidOperationException();
            }

            var scaledSize = scalingResponder.ScaleResults.ComboText;
            var location = Location;
            var spriteBatch = theaterDays.SpriteBatch;
            var destRect = RectHelper.RoundToRectangle(location.X, location.Y, scaledSize.X, scaledSize.Y);

            spriteBatch.BeginOnBufferedVisual();
            spriteBatch.Draw(_textImage, destRect, Color.White);
            spriteBatch.End();
        }

        protected override void OnLoadContents() {
            base.OnLoadContents();

            var game = Game.ToBaseGame();
            var graphics = game.GraphicsDevice;
            var config = ConfigurationStore.Get<ComboTextConfig>();

            _textImage = ContentHelper.LoadTexture(graphics, config.Data.Image.FileName);

            var clientSize = graphics.Viewport;
            var layout = config.Data.Layout;

            var x = layout.X.ToActualValue(clientSize.Width);
            var y = layout.Y.ToActualValue(clientSize.Height);

            Location = new Vector2(x, y);
        }

        protected override void OnUnloadContents() {
            _textImage.Dispose();

            base.OnUnloadContents();
        }

        private Texture2D _textImage;

    }
}
