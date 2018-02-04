using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Gaming;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Advanced;
using OpenMLTD.MilliSim.Graphics.Extensions;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Overlays {
    public class HitRankAnimation : BufferedVisual {

        public HitRankAnimation([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        public void StartAnimation(int imageIndex) {
            var syncTimer = Game.ToBaseGame().FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                throw new InvalidOperationException();
            }

            _animationStartedTime = syncTimer.CurrentTime;
            _isAnimationStarted = true;
            _selectedImageIndex = imageIndex;
        }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            var theaterDays = Game.ToBaseGame();

            var syncTimer = theaterDays.FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                throw new InvalidOperationException();
            }

            var currentTime = syncTimer.CurrentTime;
            if (currentTime < _animationStartedTime) {
                // Automatically cancels the animation if the user steps back in UI.
                _isAnimationStarted = false;
                Opacity = 0;
                return;
            }

            var animationTime = (currentTime - _animationStartedTime).TotalSeconds;

            if (animationTime > _stage1Duration + _stage2Duration) {
                Opacity = 0;
                _isAnimationStarted = false;
                return;
            }

            if (animationTime > _stage1Duration) {
                Opacity = 1 - (float)(animationTime - _stage1Duration) / (float)_stage2Duration;
            } else {
                Opacity = 1;
            }
        }

        protected override void OnDrawBuffer(GameTime gameTime) {
            base.OnDrawBuffer(gameTime);

            if (!_isAnimationStarted) {
                return;
            }

            var theaterDays = Game.ToBaseGame();

            var syncTimer = theaterDays.FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                throw new InvalidOperationException();
            }

            var scalingResponder = theaterDays.FindSingleElement<MltdStageScalingResponder>();
            if (scalingResponder == null) {
                throw new InvalidOperationException();
            }

            var tapPoints = theaterDays.FindSingleElement<TapPoints>();
            if (tapPoints == null) {
                throw new InvalidOperationException();
            }

            var currentTime = syncTimer.CurrentTime;
            if (currentTime < _animationStartedTime) {
                // Automatically cancels the animation if the user steps back in UI.
                _isAnimationStarted = false;
                return;
            }

            var animationTime = (currentTime - _animationStartedTime).TotalSeconds;

            if (animationTime > _stage1Duration + _stage2Duration) {
                Opacity = 0;
                _isAnimationStarted = false;
                return;
            }

            if (_selectedImageIndex < 0 || _selectedImageIndex >= _hitRankImages.Count) {
                return;
            }

            var clientSize = theaterDays.GraphicsDevice.Viewport;
            var scalingResults = scalingResponder.ScaleResults;
            var config = ConfigurationStore.Get<HitRankAnimationConfig>();
            var hitRankLayout = config.Data.Layout;

            var perc = (float)animationTime / (float)(_stage1Duration + _stage2Duration);
            var scale = MathHelper.Lerp(_initialScale, 1, perc);

            var centerX = hitRankLayout.X.IsPercentage ? hitRankLayout.X.Value * clientSize.Width : hitRankLayout.X.Value;
            var centerY = hitRankLayout.Y.IsPercentage ? hitRankLayout.Y.Value * clientSize.Height : hitRankLayout.Y.Value;
            var scaledWidth = scalingResults.HitRank.X * scale;
            var scaledHeight = scalingResults.HitRank.Y * scale;

            var spriteBatch = theaterDays.SpriteBatch;
            var destRect = RectHelper.RoundToRectangle(centerX - scaledWidth / 2, centerY - scaledHeight / 2, scaledWidth, scaledHeight);

            spriteBatch.BeginOnBufferedVisual();
            spriteBatch.Draw(_hitRankImages, _selectedImageIndex, destRect);
            spriteBatch.End();
        }

        protected override void OnLoadContents() {
            base.OnLoadContents();

            var config = ConfigurationStore.Get<HitRankAnimationConfig>();

            _hitRankImages = ContentHelper.LoadSpriteSheet1D(Game.GraphicsDevice, config.Data.Images.File, config.Data.Images.Count, (SpriteSheetOrientation)config.Data.Images.Orientation);
        }

        protected override void OnUnloadContents() {
            _hitRankImages.Dispose();

            base.OnUnloadContents();
        }

        private SpriteSheet1D _hitRankImages;

        private readonly float _initialScale = 1.2f;
        private readonly double _stage1Duration = 0.5;
        private readonly double _stage2Duration = 0.3;

        private int _selectedImageIndex;
        private bool _isAnimationStarted;
        private TimeSpan _animationStartedTime;

    }
}
