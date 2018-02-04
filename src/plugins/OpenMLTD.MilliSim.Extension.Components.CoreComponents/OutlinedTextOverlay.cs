using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Overlay;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public class OutlinedTextOverlay : TextOverlayBase {

        public OutlinedTextOverlay([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        public Color OutlineColor { get; set; } = Color.Black;

        public float OutlineThickness { get; set; } = 5;

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
                using (var pen = new Pen(OutlineColor, OutlineThickness)) {
                    Graphics.DrawString(pen, Font, Text, location);
                    Graphics.FillString(brush, Font, Text, location);
                }
            }
        }

    }
}
