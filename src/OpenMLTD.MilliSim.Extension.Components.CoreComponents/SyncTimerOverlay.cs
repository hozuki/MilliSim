using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;
using Point = System.Drawing.Point;

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

        protected override void OnDraw(GameTime gameTime) {
            var game = Game.ToBaseGame();
            var viewport = game.GraphicsDevice.Viewport;
            var textSize = SpriteFont.MeasureString(Text, InfiniteBounds, Vector2.One, 1, FontHelper.PointsToPixels(FontSize));

            Location = new Point((int)(viewport.Width - textSize.X), _fpsOverlayHeight);

            base.OnDraw(gameTime);
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
