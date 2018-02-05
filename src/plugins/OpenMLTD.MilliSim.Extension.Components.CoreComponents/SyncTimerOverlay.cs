using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public class SyncTimerOverlay : TextOverlay {

        public SyncTimerOverlay([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            var syncTimer = Game.ToBaseGame().FindSingleElement<SyncTimer>();
            if (syncTimer != null) {
                Text = syncTimer.CurrentTime.ToString(@"hh\:mm\:ss\.fff");
            }
        }

        protected override Vector2? MeasureText(GameTime gameTime) {
            var game = Game.ToBaseGame();
            var viewport = game.GraphicsDevice.Viewport;
            var textSize = Graphics.MeasureString(Font, Text);

            Location = new Vector2(viewport.Width - textSize.X, _fpsOverlayHeight);

            return textSize;
        }

        protected override void OnLoadContents() {
            base.OnLoadContents();

            var game = Game.ToBaseGame();

            var fps = game.FindSingleElement<FpsOverlay>();
            var fpsHeight = fps != null ? (int)fps.FontSize : 0;

            _fpsOverlayHeight = fpsHeight;
        }

        private int _fpsOverlayHeight;

    }
}
