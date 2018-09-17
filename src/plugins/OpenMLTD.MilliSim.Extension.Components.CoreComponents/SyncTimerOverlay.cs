using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    /// <inheritdoc />
    /// <summary>
    /// Displays current time on the top-right corner of the screen.
    /// </summary>
    public class SyncTimerOverlay : TextOverlay {

        /// <summary>
        /// Creates a new <see cref="SyncTimerOverlay"/> instance.
        /// </summary>
        /// <param name="game">The base game.</param>
        /// <param name="parent">The parent of the <see cref="SyncTimerOverlay"/>.</param>
        public SyncTimerOverlay([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            var syncTimer = Game.ToBaseGame().FindFirstElementOrDefault<SyncTimer>();
            if (syncTimer != null) {
                Text = syncTimer.CurrentTime.ToString(@"hh\:mm\:ss\.fff");
            }
        }

        protected override Vector2? MeasureText(GameTime gameTime) {
            var graphics = Graphics;

            if (graphics == null) {
                return null;
            }

            var game = Game.ToBaseGame();
            var viewport = game.GraphicsDevice.Viewport;
            var textSize = graphics.MeasureString(Font, Text);

            Location = new Vector2(viewport.Width - textSize.X, _fpsOverlayHeight);

            return textSize;
        }

        protected override void OnLoadContents() {
            base.OnLoadContents();

            var game = Game.ToBaseGame();

            var fps = game.FindFirstElementOrDefault<FpsOverlay>();
            var fpsHeight = fps != null ? (int)fps.FontSize : 0;

            _fpsOverlayHeight = fpsHeight;
        }

        private int _fpsOverlayHeight;

    }
}
