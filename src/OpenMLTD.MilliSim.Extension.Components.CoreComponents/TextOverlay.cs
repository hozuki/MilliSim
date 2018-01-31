using System.IO;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Text;
using MonoGame.Extended.Text.Extensions;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents.Extensions;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;
using Point = System.Drawing.Point;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    /// <summary>
    /// A basic text overlay.
    /// Suggested for dynamic texts or long texts.
    /// </summary>
    public class TextOverlay : TextOverlayBase {

        public TextOverlay([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        protected override void OnDraw(GameTime gameTime) {
            base.OnDraw(gameTime);

            var text = Text;
            var location = Location;

            var game = Game.ToBaseGame();

            game.SpriteBatch.Begin();
            game.SpriteBatch.DrawString(_spriteFont, text, new Vector2(location.X, location.Y), FillColor.ToXna());
            game.SpriteBatch.End();
        }

        protected override void OnLoadContents() {
            base.OnLoadContents();

            var fontPath = Path.GetFullPath(GetFontFilePath());

            _font = FontManager.LoadFont(fontPath, FontSize);
            _spriteFont = new DynamicSpriteFont(Game.ToBaseGame().GraphicsDevice, _font);
        }

        protected override void OnUnloadContents() {
            _spriteFont?.Dispose();
            _font?.Dispose();

            _spriteFont = null;
            _font = null;

            base.OnUnloadContents();
        }

        protected static readonly Vector2 InfiniteBounds = new Vector2(float.MaxValue, float.MaxValue);

        protected DynamicSpriteFont SpriteFont => _spriteFont;

        private Font _font;
        private DynamicSpriteFont _spriteFont;

    }
}
