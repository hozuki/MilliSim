using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public class HelpOverlay : OutlinedTextOverlay {

        public HelpOverlay([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        public override float FontSize { get; set; } = 30;

        protected override Vector2? MeasureText(GameTime gameTime) {
            var viewport = Game.ToBaseGame().GraphicsDevice.Viewport;
            var textSize = Graphics.MeasureString(Font, Text);

            var left = (viewport.Width - textSize.X) / 2;
            var top = viewport.Height * 0.75f - textSize.Y / 2;
            Location = new Vector2(left, top);

            return textSize;
        }

    }
}
