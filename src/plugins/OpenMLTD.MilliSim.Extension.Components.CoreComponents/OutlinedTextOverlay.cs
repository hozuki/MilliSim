using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Overlay;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    /// <inheritdoc />
    /// <summary>
    /// A text overlay whose text are outlined.
    /// </summary>
    public class OutlinedTextOverlay : TextOverlayBase {

        /// <inheritdoc />
        /// <summary>
        /// Creates a new <see cref="OutlinedTextOverlay"/> instance.
        /// </summary>
        /// <param name="game">The base game.</param>
        /// <param name="parent">The parent of the <see cref="OutlinedTextOverlay"/>.</param>
        public OutlinedTextOverlay([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        /// <summary>
        /// Gets or sets the color of the outline.
        /// </summary>
        public Color OutlineColor { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets the thickness of the outline, in pixels.
        /// </summary>
        public float OutlineThickness { get; set; } = 5;

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
                using (var pen = new Pen(OutlineColor, OutlineThickness)) {
                    graphics.DrawString(pen, Font, Text, location);
                    graphics.FillString(brush, Font, Text, location);
                }
            }
        }

    }
}
