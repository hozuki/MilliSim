using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.VideoPlayback;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;
using Video = MonoGame.Extended.Framework.Media.Video;
using VideoPlayer = MonoGame.Extended.Framework.Media.VideoPlayer;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    /// <inheritdoc />
    /// <summary>
    /// A background visual element displaying a video.
    /// </summary>
    public sealed class BackgroundVideo : BackgroundBase {

        /// <inheritdoc />
        /// <summary>
        /// Creates a new <see cref="BackgroundVideo"/> instance.
        /// </summary>
        /// <param name="game">The base game.</param>
        /// <param name="parent">The parent of the <see cref="BackgroundVideo"/>.</param>
        public BackgroundVideo([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        /// <summary>
        /// Gets current video playback time.
        /// </summary>
        public TimeSpan CurrentTime => _videoPlayer?.PlayPosition ?? TimeSpan.Zero;

        /// <summary>
        /// Loads a video file by path.
        /// </summary>
        /// <param name="filePath">The path to the video file.</param>
        public void Load([NotNull] string filePath) {
            _video?.Dispose();
            _video = VideoHelper.LoadFromFile(filePath);
        }

        /// <summary>
        /// Unloads current video file.
        /// </summary>
        public void Unload() {
            if (_video == null) {
                return;
            }

            _videoPlayer?.Play(null);

            _video?.Dispose();
            _video = null;
        }

        /// <summary>
        /// Starts playback.
        /// </summary>
        public void Play() {
            var videoPlayer = _videoPlayer;

            if (videoPlayer != null) {
                videoPlayer.Play(_video);
                // Yes it is still needed.
                videoPlayer.Volume = _delayedVolume;
            }
        }

        /// <summary>
        /// Stops playback.
        /// </summary>
        public void Stop() {
            _videoPlayer?.Stop();
        }

        /// <summary>
        /// Pauses playback.
        /// </summary>
        public void Pause() {
            _videoPlayer?.Pause();
        }

        /// <summary>
        /// Resumes playback.
        /// </summary>
        public void Resume() {
            _videoPlayer?.Resume();
        }

        /// <summary>
        /// Gets current state of the <see cref="BackgroundVideo"/>.
        /// </summary>
        public MediaState State => _videoPlayer?.State ?? MediaState.Stopped;

        /// <summary>
        /// Gets or sets the volume of the video.
        /// </summary>
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
