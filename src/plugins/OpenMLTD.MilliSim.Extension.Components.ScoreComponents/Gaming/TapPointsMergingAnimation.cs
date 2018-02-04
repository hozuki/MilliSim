using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Extensions;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Gaming {
    public class TapPointsMergingAnimation : Visual, IVisual2D {

        public TapPointsMergingAnimation([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        public Vector2 Location { get; set; }

        public void StartAnimation() {
            var syncTimer = Game.ToBaseGame().FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                throw new InvalidOperationException();
            }

            _animationStartedTime = syncTimer.CurrentTime;
            _isAnimationStarted = true;
        }

        protected override void OnDraw(GameTime gameTime) {
            base.OnDraw(gameTime);

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

            if (animationTime > _phase1Duration + _phase2Duration) {
                _isAnimationStarted = false;
                return;
            }

            var clientSize = theaterDays.GraphicsDevice.Viewport;
            var scalingResults = scalingResponder.ScaleResults;
            var tapPointsConfig = ConfigurationStore.Get<TapPointsConfig>();
            var tapPointsLayout = tapPointsConfig.Data.Layout;

            var spriteBatch = theaterDays.SpriteBatch;

            spriteBatch.Begin();

            float perc;
            var y = tapPointsLayout.Y.IsPercentage ? tapPointsLayout.Y.Value * clientSize.Height : tapPointsLayout.Y.Value;
            if (animationTime <= _phase1Duration) {
                perc = (float)animationTime / (float)_phase1Duration;

                var tapPointSizes = scalingResults.TapPoint;
                var auraSizes = scalingResults.SpecialNoteAura;

                var tapPointWidth = MathHelper.Lerp(tapPointSizes.Start.X, tapPointSizes.End.X, perc);
                var tapPointHeight = MathHelper.Lerp(tapPointSizes.Start.Y, tapPointSizes.End.Y, perc);
                var auraWidth = MathHelper.Lerp(auraSizes.Start.X, auraSizes.End.X, perc);
                var auraHeight = MathHelper.Lerp(auraSizes.Start.Y, auraSizes.End.Y, perc);

                var xRatioArray = tapPoints.EndXRatios;
                for (var i = 0; i < xRatioArray.Length; ++i) {
                    var x = MathHelper.Lerp(xRatioArray[i], 0.5f, perc) * clientSize.Width;
                    spriteBatch.Draw(_tapPointImage, RectHelper.RoundToRectangle(x - tapPointWidth / 2, y - tapPointHeight / 2, tapPointWidth, tapPointHeight), 1 - perc);
                    spriteBatch.Draw(_auraImage, RectHelper.RoundToRectangle(x - auraWidth / 2, y - auraHeight / 2, auraWidth, auraHeight), perc);
                }
            } else {
                perc = (float)(animationTime - _phase1Duration) / (float)_phase2Duration;

                var auraSize = scalingResults.SpecialNoteAura.End;
                var socketSize = scalingResults.SpecialNoteSocket;

                var x = clientSize.Width * 0.5f;
                spriteBatch.Draw(_socketImage, RectHelper.RoundToRectangle(x - socketSize.X / 2, y - socketSize.Y / 2, socketSize.X, socketSize.Y), perc);
                spriteBatch.Draw(_auraImage, RectHelper.RoundToRectangle(x - auraSize.X / 2, y - auraSize.Y / 2, auraSize.X, auraSize.Y));
            }

            spriteBatch.End();
        }

        protected override void OnLoadContents() {
            base.OnLoadContents();

            var notesLayerConfig = ConfigurationStore.Get<NotesLayerConfig>();
            var tapPointsConfig = ConfigurationStore.Get<TapPointsConfig>();

            var graphics = Game.GraphicsDevice;

            _auraImage = ContentHelper.LoadTexture(graphics, notesLayerConfig.Data.Images.SpecialNoteAura.FileName);
            _socketImage = ContentHelper.LoadTexture(graphics, notesLayerConfig.Data.Images.SpecialNoteSocket.FileName);
            _tapPointImage = ContentHelper.LoadTexture(graphics, tapPointsConfig.Data.Images.TapPoint.FileName);
        }

        protected override void OnUnloadContents() {
            _auraImage.Dispose();
            _socketImage.Dispose();
            _tapPointImage.Dispose();

            base.OnUnloadContents();
        }

        // Total time: 0.8s (corresponding to the advancing time of Special Prepare)
        private readonly double _phase1Duration = 0.5;
        private readonly double _phase2Duration = 0.3;

        private Texture2D _auraImage;
        private Texture2D _socketImage;
        private Texture2D _tapPointImage;

        private bool _isAnimationStarted;
        private TimeSpan _animationStartedTime;

    }
}
