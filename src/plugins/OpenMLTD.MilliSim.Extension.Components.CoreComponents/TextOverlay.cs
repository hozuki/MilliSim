using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Overlay;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    /// <inheritdoc />
    /// <summary>
    /// A basic text overlay.
    /// Suggested for dynamic texts or long texts.
    /// </summary>
    public class TextOverlay : TextOverlayBase {

        /// <inheritdoc />
        /// <summary>
        /// Creates a new <see cref="TextOverlay"/> instance.
        /// </summary>
        /// <param name="game">The base game.</param>
        /// <param name="parent">The parent of the <see cref="TextOverlay"/>.</param>
        public TextOverlay([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        protected override void OnDrawText(GameTime gameTime, Vector2? measuredSize) {
            var graphics = Graphics;

            if (graphics == null) {
                return;
            }

            Vector2 size;

            if (measuredSize != null) {
                size = measuredSize.Value;
            } else {
                size = graphics.MeasureString(Font, Text);
            }

            var location = Location;

            location.Y += size.Y;

            using (var brush = new SolidBrush(FillColor)) {
                graphics.FillMultilineString(brush, Font, Text, location);
            }
        }

    }
}
