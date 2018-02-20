using System;
using System.IO;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration;
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

            var texture = _texture;

            if (texture != null) {
                var game = Game.ToBaseGame();
                var config = ConfigurationStore.Get<BackgroundImageConfig>();

                var samplerState = config.Data.Fit == BackgroundImageFit.Tile ? SamplerState.LinearWrap : SamplerState.LinearClamp;
                var viewport = game.GraphicsDevice.Viewport;

                game.SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: samplerState);

                Rectangle? destRect = null;

                switch (config.Data.Fit) {
                    case BackgroundImageFit.None:
                        break;
                    case BackgroundImageFit.Fit: {
                            var viewportAspectRatio = viewport.AspectRatio;
                            var textureAspectRatio = (float)texture.Width / texture.Height;

                            if (viewportAspectRatio > textureAspectRatio) {
                                // Width is the limitation
                                destRect = RectHelper.RoundToRectangle(0, (float)(viewport.Height - texture.Height) / 2, viewport.Width, viewport.Width / textureAspectRatio);
                            } else {
                                // Height is the limitation
                                destRect = RectHelper.RoundToRectangle((float)(viewport.Width - texture.Width) / 2, 0, viewport.Height * textureAspectRatio, viewport.Height);
                            }
                        }
                        break;
                    case BackgroundImageFit.Stretch:
                        destRect = viewport.Bounds;
                        break;
                    case BackgroundImageFit.Tile:
                        break;
                    case BackgroundImageFit.Center:
                        destRect = RectHelper.RoundToRectangle((float)(viewport.Width - texture.Width) / 2, (float)(viewport.Height - texture.Height) / 2, texture.Width, texture.Height);
                        break;
                    case BackgroundImageFit.LetterBox: {
                            var viewportAspectRatio = viewport.AspectRatio;
                            var textureAspectRatio = (float)texture.Width / texture.Height;

                            if (viewportAspectRatio > textureAspectRatio) {
                                // Width is the limitation
                                var newWidth = viewport.Height * textureAspectRatio;
                                destRect = RectHelper.RoundToRectangle((viewport.Width - newWidth) / 2, 0, newWidth, viewport.Height);
                            } else {
                                // Height is the limitation
                                var newHeight = viewport.Width / textureAspectRatio;
                                destRect = RectHelper.RoundToRectangle(0, (viewport.Height - newHeight) / 2, viewport.Width, newHeight);
                            }
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (destRect != null) {
                    game.SpriteBatch.Draw(texture, destRect.Value, Color.White);
                } else {
                    game.SpriteBatch.Draw(texture, Vector2.Zero, Color.White);
                }

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
