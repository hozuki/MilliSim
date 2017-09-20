using System;
using System.Drawing;
using System.Linq;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D.Effects;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Theater.Elements.Logical;
using OpenMLTD.MilliSim.Theater.Extensions;
using SharpDX.Direct2D1;
using RectangleF = System.Drawing.RectangleF;

namespace OpenMLTD.MilliSim.Theater.Elements.Visual.Gaming {
    public class TapPoints : BufferedElement2D {

        public TapPoints(GameBase game)
            : base(game) {
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
            var syncTimer = Game.AsTheaterDays().FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                throw new InvalidOperationException();
            }

            _animationStartedTime = syncTimer.CurrentTime;
            _ongoingAnimation = OngoingAnimation.ScorePrepare;
        }

        public void PlaySpecialEndAnimation() {
            var syncTimer = Game.AsTheaterDays().FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                throw new InvalidOperationException();
            }

            _animationStartedTime = syncTimer.CurrentTime;
            _ongoingAnimation = OngoingAnimation.SpecialEnd;
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
                var score = theaterDays.FindSingleElement<ScoreLoader>()?.RuntimeScore;
                if (score == null) {
                    throw new InvalidOperationException();
                }

                var now = currentTime.TotalSeconds;
                var scorePrepareNote = score.Notes.Single(n => n.Type == NoteType.ScorePrepare);
                var specialPrepareNote = score.Notes.Single(n => n.Type == NoteType.SpecialPrepare);
                var specialEndNote = score.Notes.Single(n => n.Type == NoteType.SpecialEnd);

                if (now < scorePrepareNote.HitTime || (specialPrepareNote.HitTime < now && now < specialEndNote.HitTime)) {
                    Opacity = 0;
                } else {
                    // Automatically cancels the animation if the user steps back in UI.
                    Opacity = 1;
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

        protected override void OnLayout() {
            base.OnLayout();

            var settings = Program.Settings;

            var tapPointsLayout = settings.UI.TapPoints.Layout;
            float workingAreaWidthRatio;
            if (tapPointsLayout.Width.IsPercentage) {
                workingAreaWidthRatio = tapPointsLayout.Width.Value;
            } else {
                var workingAreaWidth = tapPointsLayout.Width.Value;
                // TODO: we should get RenderContext.ClientSize instead of using user-specified client size. Current method does not seems contain a method to know this.
                // Suggested method: replace Game with Root-Parent in constructor.
                workingAreaWidthRatio = workingAreaWidth / settings.Window.Width;
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

        protected override void OnDrawBuffer(GameTime gameTime, RenderContext context) {
            base.OnDrawBuffer(gameTime, context);

            if (Opacity <= 0) {
                return;
            }

            var settings = Program.Settings;
            var clientSize = context.ClientSize;
            var tapPointsLayout = settings.UI.TapPoints.Layout;

            float centerY;
            if (tapPointsLayout.Y.IsPercentage) {
                centerY = tapPointsLayout.Y.Value * clientSize.Height;
            } else {
                centerY = tapPointsLayout.Y.Value;
            }

            float[] tapPointXPercArray = _tapPointsX, nodeXPercArray = _tapNodesX;

            var gamingArea = Game.AsTheaterDays().FindSingleElement<GamingArea>();

            if (gamingArea == null) {
                throw new InvalidOperationException();
            }

            var scaleResults = gamingArea.ScaleResults;

            // Draw "chains", left and right.
            context.PushTransform2D();
            context.Translate2D(0, centerY - scaleResults.TapBarChain.Height / 2);
            context.Begin2D();
            var yy = scaleResults.TapBarChain.Height / 2;
            var blankLeft = settings.Images.TapPoint.BlankEdge.Left;
            var blankRight = settings.Images.TapPoint.BlankEdge.Right;
            for (var i = 0; i < nodeXPercArray.Length; ++i) {
                var x1 = clientSize.Width * tapPointXPercArray[i];
                var x2 = clientSize.Width * nodeXPercArray[i];
                x1 += scaleResults.TapPoint.Start.Width / 2 - blankLeft;
                x2 -= scaleResults.TapBarNode.Width / 2;
                // Already translated.
                context.DrawLine(_tapBarChainPen, x1, yy, x2, yy);

                x1 = clientSize.Width * nodeXPercArray[i];
                x2 = clientSize.Width * tapPointXPercArray[i + 1];
                x1 += scaleResults.TapBarNode.Width / 2;
                x2 -= scaleResults.TapPoint.Start.Width / 2 - blankRight;
                // Already translated.
                context.DrawLine(_tapBarChainPen, x1, yy, x2, yy);
            }
            context.End2D();
            context.PopTransform2D();

            // Draw nodes.
            context.Begin2D();
            for (var i = 0; i < nodeXPercArray.Length; ++i) {
                var centerX = clientSize.Width * nodeXPercArray[i];
                context.DrawBitmap(_tapBarNodeImage, centerX - scaleResults.TapBarNode.Width / 2, centerY - scaleResults.TapBarNode.Height / 2, scaleResults.TapBarNode.Width, scaleResults.TapBarNode.Height);
            }
            context.End2D();

            // Draw tap areas.
            context.Begin2D();
            for (var i = 0; i < tapPointXPercArray.Length; ++i) {
                var centerX = clientSize.Width * tapPointXPercArray[i];
                context.DrawBitmap(_tapPointImage, centerX - scaleResults.TapPoint.Start.Width / 2, centerY - scaleResults.TapPoint.Start.Height / 2, scaleResults.TapPoint.Start.Width, scaleResults.TapPoint.Start.Height);
            }
            context.End2D();
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);

            var settings = Program.Settings;
            var gamingArea = Game.AsTheaterDays().FindSingleElement<GamingArea>();

            if (gamingArea == null) {
                throw new InvalidOperationException();
            }

            //Opacity = settings.UI.TapPoints.Opacity.Value;

            _tapPointImage = Direct2DHelper.LoadBitmap(context, settings.Images.TapPoint.FileName);

            var tapBarChainImage = Direct2DHelper.LoadBitmap(context, settings.Images.TapBarChain.FileName);
            _tapBarChainImage = tapBarChainImage;

            _tapBarNodeImage = Direct2DHelper.LoadBitmap(context, settings.Images.TapBarNode.FileName);

            var scaledTapBarChainSize = gamingArea.ScaleResults.TapBarChain;
            var tapBarChainScaledRatio = new SizeF(scaledTapBarChainSize.Width / tapBarChainImage.Width, scaledTapBarChainSize.Height / tapBarChainImage.Height);
            _tapBarChainScaleEffect = new D2DScaleEffect(context) {
                Scale = tapBarChainScaledRatio
            };
            _tapBarChainScaleEffect.SetInput(0, tapBarChainImage);

            var tapBarBrushRect = new RectangleF(0, 0, scaledTapBarChainSize.Width, scaledTapBarChainSize.Height);
            _tapBarChainBrush = new D2DImageBrush(context, _tapBarChainScaleEffect, ExtendMode.Wrap, ExtendMode.Wrap, InterpolationMode.Linear, tapBarBrushRect);
            _tapBarChainPen = new D2DPen(_tapBarChainBrush, scaledTapBarChainSize.Height);
        }

        protected override void OnLostContext(RenderContext context) {
            base.OnLostContext(context);
            _tapBarChainPen.Dispose();
            _tapBarChainBrush.Dispose();
            _tapBarChainScaleEffect.Dispose();
            _tapPointImage?.Dispose();
            _tapBarChainImage?.Dispose();
            _tapBarNodeImage?.Dispose();
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            Opacity = 0;
        }

        private float[] _tapPointsX;
        private float[] _incomingX;

        private float[] _tapNodesX;

        private OngoingAnimation _ongoingAnimation = OngoingAnimation.None;
        private TimeSpan _animationStartedTime = TimeSpan.Zero;

        private readonly double _scorePrepareDuration = 1.5;
        private readonly double _specialEndDuration = 1.5;

        [CanBeNull]
        private D2DBitmap _tapPointImage;
        [CanBeNull]
        private D2DBitmap _tapBarChainImage;
        [CanBeNull]
        private D2DBitmap _tapBarNodeImage;

        private D2DScaleEffect _tapBarChainScaleEffect;

        private D2DImageBrush _tapBarChainBrush;
        private D2DPen _tapBarChainPen;

        private int _trackCount = 2;

        private enum OngoingAnimation {

            None = 0,
            ScorePrepare = 1,
            SpecialEnd = 2

        }

    }
}
