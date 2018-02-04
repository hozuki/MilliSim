using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Framework.Media;
using MonoGame.Extended.VideoPlayback;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;
using Video = MonoGame.Extended.Framework.Media.Video;
using VideoPlayer = MonoGame.Extended.Framework.Media.VideoPlayer;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public sealed class BackgroundVideo : BackgroundBase {

        public BackgroundVideo([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        public TimeSpan CurrentTime => _videoPlayer?.PlayPosition ?? TimeSpan.Zero;

        public void Load([NotNull] string filePath) {
            _video?.Dispose();
            _video = VideoHelper.LoadFromFile(filePath);
        }

        public void Unload() {
            if (_video == null) {
                return;
            }

            _videoPlayer?.Play(null);

            _video?.Dispose();
            _video = null;
        }

        public void Play() {
            var videoPlayer = _videoPlayer;

            if (videoPlayer != null) {
                videoPlayer.Play(_video);
                // Yes it is still needed.
                videoPlayer.Volume = _delayedVolume;
            }
        }

        public void Stop() {
            _videoPlayer?.Stop();
        }

        public void Pause() {
            _videoPlayer?.Pause();
        }

        public void Resume() {
            _videoPlayer?.Resume();
        }

        public MediaState State => _videoPlayer?.State ?? MediaState.Stopped;

        public float Volume {
            get => _delayedVolume;
            set {
                if (_videoPlayer != null) {
                    _videoPlayer.Volume = value;
                }

                _delayedVolume = value;
            }
        }

        protected override void OnDraw(GameTime gameTime) {
            base.OnDraw(gameTime);

            var texture = _videoPlayer?.GetTexture();

            if (texture == null) {
                return;
            }

            var game = Game.ToBaseGame();
            var spriteBatch = game.SpriteBatch;
            var viewport = game.GraphicsDevice.Viewport;

            spriteBatch.Begin(blendState: BlendState.AlphaBlend);
            spriteBatch.Draw(texture, viewport.Bounds, Color.White);
            spriteBatch.End();

            texture.Dispose();
        }

        protected override void OnLoadContents() {
            base.OnLoadContents();

            _videoPlayer = new VideoPlayer(Game.GraphicsDevice);
            _videoPlayer.Volume = _delayedVolume;
        }

        protected override void OnUnloadContents() {
            Unload();

            _videoPlayer?.Dispose();
            _videoPlayer = null;

            base.OnUnloadContents();
        }

        [CanBeNull]
        private VideoPlayer _videoPlayer;
        [CanBeNull]
        private Video _video;

        private float _delayedVolume = 1f;

    }
}
