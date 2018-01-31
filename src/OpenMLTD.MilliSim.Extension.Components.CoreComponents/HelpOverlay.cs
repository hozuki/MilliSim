using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;
using Point = System.Drawing.Point;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public class HelpOverlay : TextOverlay {

        public HelpOverlay([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        public override float FontSize { get; set; } = 30;

        protected override void OnDraw(GameTime gameTime) {
            var viewport = Game.ToBaseGame().GraphicsDevice.Viewport;
            var textSize = SpriteFont.MeasureString(Text, InfiniteBounds, Vector2.One, 1, FontSize);

            var left = (viewport.Width - textSize.X) / 2;
            var top = viewport.Height * 0.75f - textSize.Y / 2;
            Location = new Point((int)left, (int)top);

            base.OnDraw(gameTime);
        }

    }
}
