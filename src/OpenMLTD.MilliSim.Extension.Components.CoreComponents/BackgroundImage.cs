using System.IO;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public sealed class BackgroundImage : BackgroundBase {

        public BackgroundImage([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        public void Load([CanBeNull] string path) {
            if (path == _filePath) {
                return;
            }

            _texture?.Dispose();
            _texture = null;

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) {
                return;
            }

            // Delay load.
            _filePath = path;
        }

        public void Unload() {
            _filePath = null;
        }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            if (_filePath != null) {
                if (_texture == null) {
                    var game = Game;
                    _texture = ContentHelper.LoadTexture(game.GraphicsDevice, _filePath);
                }
            } else {
                if (_texture != null) {
                    _texture.Dispose();
                    _texture = null;
                }
            }
        }

        protected override void OnDraw(GameTime gameTime) {
            base.OnDraw(gameTime);

            if (_texture != null) {
                var game = Game.ToBaseGame();

                game.SpriteBatch.Begin();
                game.SpriteBatch.Draw(_texture, Vector2.Zero, Color.White);
                game.SpriteBatch.End();
            }
        }

        protected override void OnUnloadContents() {
            _texture?.Dispose();
            _texture = null;

            base.OnUnloadContents();
        }

        private string _filePath;

        private Texture2D _texture;

    }
}
