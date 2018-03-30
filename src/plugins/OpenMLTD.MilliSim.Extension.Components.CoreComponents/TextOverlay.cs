using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Overlay;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    /// <summary>
    /// A basic text overlay.
    /// Suggested for dynamic texts or long texts.
    /// </summary>
    public class TextOverlay : TextOverlayBase {

        public TextOverlay([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        protected override void OnDrawText(GameTime gameTime, Vector2? measuredSize) {
            Vector2 size;

            if (measuredSize != null) {
                size = measuredSize.Value;
            } else {
                size = Graphics.MeasureString(Font, Text);
            }

            var location = Location;

            location.Y += size.Y;

            using (var brush = new SolidBrush(FillColor)) {
                Graphics.FillMultilineString(brush, Font, Text, location);
            }
        }

    }
}
