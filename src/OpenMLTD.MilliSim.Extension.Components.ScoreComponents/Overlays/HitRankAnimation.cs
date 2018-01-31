using System;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Gaming;
using OpenMLTD.MilliSim.GameAbstraction.Extensions;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D.Advanced;
using OpenMLTD.MilliSim.Graphics.Extensions;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Overlays {
    public class HitRankAnimation : BufferedVisual2D {

        public HitRankAnimation([NotNull] IVisualContainer parent)
            : base(parent) {
        }

        public void StartAnimation(int imageIndex) {
            var syncTimer = Game.AsTheaterDays().FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                throw new InvalidOperationException();
            }

            _animationStartedTime = syncTimer.CurrentTime;
            _isAnimationStarted = true;
            _selectedImageIndex = imageIndex;
        }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            var theaterDays = Game.AsTheaterDays();

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

        protected override void OnDrawBuffer(GameTime gameTime, RenderContext context) {
            base.OnDrawBuffer(gameTime, context);

            if (!_isAnimationStarted) {
                return;
            }

            var theaterDays = Game.AsTheaterDays();

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

            var clientSize = context.ClientSize;
            var scalingResults = scalingResponder.ScaleResults;
            var config = ConfigurationStore.Get<HitRankAnimationConfig>();
            var hitRankLayout = config.Data.Layout;

            var perc = (float)animationTime / (float)(_stage1Duration + _stage2Duration);
            var scale = MathHelper.Lerp(_initialScale, 1, perc);

            var centerX = hitRankLayout.X.IsPercentage ? hitRankLayout.X.Value * clientSize.Width : hitRankLayout.X.Value;
            var centerY = hitRankLayout.Y.IsPercentage ? hitRankLayout.Y.Value * clientSize.Height : hitRankLayout.Y.Value;
            var scaledWidth = scalingResults.HitRank.Width * scale;
            var scaledHeight = scalingResults.HitRank.Height * scale;

            context.Begin2D();
            context.DrawImageStripUnit(_hitRankImages, _selectedImageIndex, centerX - scaledWidth / 2, centerY - scaledHeight / 2, scaledWidth, scaledHeight);
            context.End2D();
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);

            var config = ConfigurationStore.Get<HitRankAnimationConfig>();

            _hitRankImages = Direct2DHelper.LoadImageStrip(context, config.Data.Images.File, config.Data.Images.Count, (ImageStripOrientation)config.Data.Images.Orientation);
        }

        protected override void OnLostContext(RenderContext context) {
            _hitRankImages.Dispose();

            base.OnLostContext(context);
        }

        private D2DImageStrip _hitRankImages;

        private readonly float _initialScale = 1.2f;
        private readonly double _stage1Duration = 0.5;
        private readonly double _stage2Duration = 0.3;

        private int _selectedImageIndex;
        private bool _isAnimationStarted;
        private TimeSpan _animationStartedTime;

    }
}
