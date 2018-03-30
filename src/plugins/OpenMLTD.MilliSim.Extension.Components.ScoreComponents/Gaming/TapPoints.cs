using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Contributed.Scores;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Extensions;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Gaming {
    public class TapPoints : BufferedVisual {

        public TapPoints([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        public int TrackCount {
            get => _trackCount;
            set {
                if (value < 1) {
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }

                _trackCount = value;
            }
        }

        public float[] EndXRatios => _tapPointsX;

        public float[] StartXRatios => _incomingX;

        public void PlayScorePrepareAnimation() {
            var syncTimer = Game.ToBaseGame().FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                throw new InvalidOperationException();
            }

            _animationStartedTime = syncTimer.CurrentTime;
            _ongoingAnimation = OngoingAnimation.ScorePrepare;
        }

        public void PlaySpecialEndAnimation() {
            var syncTimer = Game.ToBaseGame().FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                throw new InvalidOperationException();
            }

            _animationStartedTime = syncTimer.CurrentTime;
            _ongoingAnimation = OngoingAnimation.SpecialEnd;
        }

        internal void RecalcLayout() {
            var viewport = Game.GraphicsDevice.Viewport;
            var clientSize = new Vector2(viewport.Width, viewport.Height);

            PerformLayout(clientSize);
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
                var score = theaterDays.FindSingleElement<ScoreLoader>()?.RuntimeScore;
                if (score == null) {
                    throw new InvalidOperationException();
                }

                var now = currentTime.TotalSeconds;
                var scorePrepareNote = score.Notes.Single(n => n.Type == NoteType.ScorePrepare);
                var specialPrepareNote = score.Notes.SingleOrDefault(n => n.Type == NoteType.SpecialPrepare);
                var specialEndNote = score.Notes.SingleOrDefault(n => n.Type == NoteType.SpecialEnd);

                if (specialPrepareNote != null && specialEndNote != null) {
                    if (now < scorePrepareNote.HitTime || (specialPrepareNote.HitTime < now && now < specialEndNote.HitTime)) {
                        Opacity = 0;
                    } else {
                        // Automatically cancels the animation if the user steps back in UI.
                        Opacity = 1;
                    }
                } else {
                    if (now < scorePrepareNote.HitTime) {
                        Opacity = 0;
                    } else {
                        Opacity = 1;
                    }
                }

                _ongoingAnimation = OngoingAnimation.None;
                // Fix suddent appearance.
                _animationStartedTime = currentTime;

                return;
            }

            if (_ongoingAnimation == OngoingAnimation.None) {
                return;
            }

            var animationTime = (currentTime - _animationStartedTime).TotalSeconds;

            switch (_ongoingAnimation) {
                case OngoingAnimation.ScorePrepare:
                    if (animationTime > _scorePrepareDuration) {
                        Opacity = 1;
                        _ongoingAnimation = OngoingAnimation.None;
                        return;
                    }

                    break;
                case OngoingAnimation.SpecialEnd:
                    if (animationTime > _specialEndDuration) {
                        Opacity = 1;
                        _ongoingAnimation = OngoingAnimation.None;
                        return;
                    }

                    break;
                case OngoingAnimation.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            float perc;
            switch (_ongoingAnimation) {
                case OngoingAnimation.ScorePrepare:
                    perc = (float)animationTime / (float)_scorePrepareDuration;
                    Opacity = perc;
                    break;
                case OngoingAnimation.SpecialEnd:
                    perc = (float)animationTime / (float)_specialEndDuration;
                    Opacity = perc;
                    break;
                case OngoingAnimation.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnDrawBuffer(GameTime gameTime) {
            base.OnDrawBuffer(gameTime);

            if (Opacity <= 0) {
                return;
            }

            var theaterDays = Game.ToBaseGame();

            var clientSize = theaterDays.GraphicsDevice.Viewport;

            var config = ConfigurationStore.Get<TapPointsConfig>();
            var tapPointsLayout = config.Data.Layout;

            float centerY;
            if (tapPointsLayout.Y.IsPercentage) {
                centerY = tapPointsLayout.Y.Value * clientSize.Height;
            } else {
                centerY = tapPointsLayout.Y.Value;
            }

            float[] tapPointXPercArray = _tapPointsX, nodeXPercArray = _tapNodesX;

            var scalingResponder = theaterDays.FindSingleElement<MltdStageScalingResponder>();

            if (scalingResponder == null) {
                throw new InvalidOperationException();
            }

            var scaleResults = scalingResponder.ScaleResults;

            var spriteBatch = theaterDays.SpriteBatch;
            var transform = Matrix.CreateTranslation(0, centerY - scaleResults.TapBarChain.Y / 2, 0);

            // Draw "chains", left and right.
            var yy = scaleResults.TapBarChain.Y / 2;
            var blankLeft = config.Data.Images.TapPoint.BlankEdge.Left;
            var blankRight = config.Data.Images.TapPoint.BlankEdge.Right;

            var tapBarChainImage = _tapBarChainImage;
            var tapBarChainSize = _tapBarChainDesiredSize;

            if (tapBarChainImage != null) {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, transformMatrix: transform, samplerState: SamplerState.LinearWrap);

                for (var i = 0; i < nodeXPercArray.Length; ++i) {
                    var x1 = clientSize.Width * tapPointXPercArray[i];
                    var x2 = clientSize.Width * nodeXPercArray[i];
                    x1 += scaleResults.TapPoint.Start.X / 2 - blankLeft;
                    x2 -= scaleResults.TapBarNode.X / 2;
                    // Already translated.
                    var destRect = RectHelper.RoundToRectangle(x1, yy - tapBarChainSize.Y / 2, x2 - x1, tapBarChainSize.Y);
                    spriteBatch.Draw(tapBarChainImage, destRect);

                    x1 = clientSize.Width * nodeXPercArray[i];
                    x2 = clientSize.Width * tapPointXPercArray[i + 1];
                    x1 += scaleResults.TapBarNode.X / 2;
                    x2 -= scaleResults.TapPoint.Start.X / 2 - blankRight;
                    // Already translated.
                    destRect = RectHelper.RoundToRectangle(x1, yy - tapBarChainSize.Y / 2, x2 - x1, tapBarChainSize.Y);
                    spriteBatch.Draw(tapBarChainImage, destRect);
                }

                spriteBatch.End();
            }

            // Draw nodes.
            var tapBarNodeImage = _tapBarNodeImage;

            if (tapBarNodeImage != null) {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                for (var i = 0; i < nodeXPercArray.Length; ++i) {
                    var centerX = clientSize.Width * nodeXPercArray[i];
                    spriteBatch.Draw(tapBarNodeImage, RectHelper.RoundToRectangle(centerX - scaleResults.TapBarNode.X / 2, centerY - scaleResults.TapBarNode.Y / 2, scaleResults.TapBarNode.X, scaleResults.TapBarNode.Y));
                }

                spriteBatch.End();
            }

            // Draw tap areas.
            if (_tapPointImage != null) {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                for (var i = 0; i < tapPointXPercArray.Length; ++i) {
                    var centerX = clientSize.Width * tapPointXPercArray[i];
                    spriteBatch.Draw(_tapPointImage, RectHelper.RoundToRectangle(centerX - scaleResults.TapPoint.Start.X / 2, centerY - scaleResults.TapPoint.Start.Y / 2, scaleResults.TapPoint.Start.X, scaleResults.TapPoint.Start.Y));
                }

                spriteBatch.End();
            }
        }

        protected override void OnLoadContents() {
            base.OnLoadContents();

            var theaterDays = Game.ToBaseGame();

            var config = ConfigurationStore.Get<TapPointsConfig>();

            var responder = theaterDays.FindSingleElement<MltdStageScalingResponder>();

            if (responder == null) {
                throw new InvalidOperationException();
            }

            //Opacity = settings.UI.TapPoints.Opacity.Value;

            _tapPointImage = ContentHelper.LoadTexture(theaterDays.GraphicsDevice, config.Data.Images.TapPoint.FileName);

            var tapBarChainImage = ContentHelper.LoadTexture(theaterDays.GraphicsDevice, config.Data.Images.TapBarChain.FileName);
            _tapBarChainImage = tapBarChainImage;

            _tapBarNodeImage = ContentHelper.LoadTexture(theaterDays.GraphicsDevice, config.Data.Images.TapBarNode.FileName);

            _tapBarChainDesiredSize = responder.ScaleResults.TapBarChain;

            //            var scaledTapBarChainSize = responder.ScaleResults.TapBarChain;
            //            var tapBarChainScaledRatio = new Vector2(scaledTapBarChainSize.Width / tapBarChainImage.Width, scaledTapBarChainSize.Height / tapBarChainImage.Height);
            //            _tapBarChainScaleEffect = new D2DScaleEffect(context) {
            //                Scale = tapBarChainScaledRatio
            //            };
            //            _tapBarChainScaleEffect.SetInput(0, tapBarChainImage);
            //
            //            var tapBarBrushRect = new RectangleF(0, 0, scaledTapBarChainSize.Width, scaledTapBarChainSize.Height);
            //            _tapBarChainBrush = new D2DImageBrush(context, _tapBarChainScaleEffect, ExtendMode.Wrap, ExtendMode.Wrap, InterpolationMode.Linear, tapBarBrushRect);
            //            _tapBarChainPen = new D2DPen(_tapBarChainBrush, scaledTapBarChainSize.Height);
        }

        protected override void OnUnloadContents() {
            base.OnUnloadContents();
            _tapPointImage?.Dispose();
            _tapBarChainImage?.Dispose();
            _tapBarNodeImage?.Dispose();
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            RecalcLayout();

            Opacity = 0;
        }

        private void PerformLayout(Vector2 clientSize) {
            var config = ConfigurationStore.Get<TapPointsConfig>();

            var scoreLoader = Game.ToBaseGame().FindSingleElement<ScoreLoader>();

            if (scoreLoader?.RuntimeScore != null) {
                TrackCount = scoreLoader.RuntimeScore.TrackCount;
            }

            var tapPointsLayout = config.Data.Layout;
            float workingAreaWidthRatio;
            if (tapPointsLayout.Width.IsPercentage) {
                workingAreaWidthRatio = tapPointsLayout.Width.Value;
            } else {
                var workingAreaWidth = tapPointsLayout.Width.Value;
                // Suggested method: replace Game with Root-Parent in constructor.
                workingAreaWidthRatio = workingAreaWidth / clientSize.X;
            }

            var l = (1 - workingAreaWidthRatio) / 2;
            var r = l + workingAreaWidthRatio;
            var n = TrackCount;

            var tracks = new float[n];
            for (var i = 0; i < n; ++i) {
                tracks[i] = l + (r - l) * i / (n - 1);
            }

            var midPoints = new float[tracks.Length - 1];
            for (var i = 0; i < midPoints.Length; ++i) {
                midPoints[i] = (tracks[i] + tracks[i + 1]) / 2;
            }

            _tapPointsX = tracks;
            _tapNodesX = midPoints;

            var incomings = new float[tracks.Length];
            for (var i = 0; i < incomings.Length; ++i) {
                incomings[i] = 0.5f + (tracks[i] - 0.5f) * 0.6f;
            }

            _incomingX = incomings;
        }

        private float[] _tapPointsX;
        private float[] _incomingX;

        private float[] _tapNodesX;

        private OngoingAnimation _ongoingAnimation = OngoingAnimation.None;
        private TimeSpan _animationStartedTime = TimeSpan.Zero;

        private readonly double _scorePrepareDuration = 1.5;
        private readonly double _specialEndDuration = 1.5;

        [CanBeNull]
        private Texture2D _tapPointImage;

        [CanBeNull]
        private Texture2D _tapBarChainImage;

        [CanBeNull]
        private Texture2D _tapBarNodeImage;

        private Vector2 _tapBarChainDesiredSize;

        private int _trackCount = 2;

        private enum OngoingAnimation {

            None = 0,
            ScorePrepare = 1,
            SpecialEnd = 2

        }

    }
}
