using System;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Overlay;
using OpenMLTD.MilliSim.Contributed.Scores;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Overlays {
    public class AvatarDisplay : OverlayBase {

        public AvatarDisplay([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

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
                var specialNote = score.Notes.Single(n => n.Type == NoteType.Special);
                var specialEndNote = score.Notes.Single(n => n.Type == NoteType.SpecialEnd);

                if (now < scorePrepareNote.HitTime || (specialNote.HitTime < now && now < specialEndNote.HitTime)) {
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

        protected override void OnLoadContents() {
            base.OnLoadContents();

            var theaterDays = Game.ToBaseGame();

            var config = ConfigurationStore.Get<AvatarDisplayConfig>();

            var avatarCount = config.Data.Images.Length;
            var avatarImages = new Texture2D[avatarCount];

            for (var i = 0; i < avatarCount; ++i) {
                try {
                    var image = ContentHelper.LoadTexture(theaterDays.GraphicsDevice, config.Data.Images[i].FileName);
                    avatarImages[i] = image;
                } catch (Exception ex) {
                    Debug.Print(ex.Message);
                }
            }

            var clientSize = theaterDays.GraphicsDevice.Viewport;
            var layout = config.Data.Layout;
            var x = layout.X.IsPercentage ? layout.X.Value * clientSize.Width : layout.X.Value;
            var y = layout.Y.IsPercentage ? layout.Y.Value * clientSize.Height : layout.Y.Value;
            var w = layout.Width.IsPercentage ? layout.Width.Value * clientSize.Width : layout.Width.Value;
            var h = layout.Height.IsPercentage ? layout.Height.Value * clientSize.Height : layout.Height.Value;

            var rects = new Rectangle[avatarCount];
            float radius;
            if (w >= h) {
                radius = h / 2;
                for (var i = 0; i < avatarCount; ++i) {
                    var cx = (w - radius * 2) / (avatarCount - 1) * i;
                    var rect = RectHelper.RoundToRectangle(x + cx, y, radius * 2, radius * 2);
                    rects[i] = rect;
                }
            } else {
                radius = w / 2;
                for (var i = 0; i < avatarCount; ++i) {
                    var cy = (h - radius * 2) / (avatarCount - 1) * i;
                    var rect = RectHelper.RoundToRectangle(x, y + cy, radius * 2, radius * 2);
                    rects[i] = rect;
                }
            }

            _avatarImages = avatarImages;
            _avatarRectangles = rects;

            var avatarImagesData = new byte[avatarImages.Length][];

            for (var i = 0; i < avatarImages.Length; ++i) {
                if (avatarImages[i] == null) {
                    continue;
                }

                var data = new byte[avatarImages[i].Width * avatarImages[i].Height * sizeof(int)];
                avatarImages[i].GetData(data);

                avatarImagesData[i] = data;
            }

            _avatarImagesData = avatarImagesData;

            var scalingResponder = theaterDays.FindSingleElement<MltdStageScalingResponder>();

            if (scalingResponder == null) {
                throw new InvalidOperationException();
            }

            var scalingResults = scalingResponder.ScaleResults;

            _borderColor = Color.White;
            _borderWidth = scalingResults.AvatarBorder.X;

            DrawContents();
        }

        protected override void Dispose(bool disposing) {
            foreach (var image in _avatarImages) {
                image?.Dispose();
            }

            _avatarImages = null;

            base.Dispose(disposing);
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            Opacity = 0;
        }

        // We only need to draw it once; SkiaSharp use retained graphics.
        private void DrawContents() {
            var avatarRectangles = _avatarRectangles;
            var graphics = Graphics;

            var clipPath = new Path();

            foreach (var avatarRect in avatarRectangles) {
                clipPath.AddEllipse(avatarRect);
            }

            graphics.SaveState();

            graphics.SetClipPath(clipPath, false);

            var avatarImages = _avatarImages;
            var avatarImagesData = _avatarImagesData;

            for (var i = 0; i < avatarImages.Length; ++i) {
                var image = avatarImages[i];

                if (image == null) {
                    continue;
                }

                var imageData = avatarImagesData[i];

                Debug.Assert(imageData != null, nameof(imageData) + "!= null");

                var rect = avatarRectangles[i];

                graphics.DrawImage(imageData, image.Format, image.Width, image.Height, rect.X, rect.Y, rect.Width, rect.Height);
            }

            graphics.RestoreState();

            using (var borderPen = new Pen(_borderColor, _borderWidth)) {
                foreach (var rect in avatarRectangles) {
                    graphics.DrawEllipse(borderPen, rect);
                }
            }
        }

        private Rectangle[] _avatarRectangles;

        [ItemCanBeNull]
        private Texture2D[] _avatarImages;

        [ItemCanBeNull]
        private byte[][] _avatarImagesData;

        private OngoingAnimation _ongoingAnimation = OngoingAnimation.None;
        private TimeSpan _animationStartedTime = TimeSpan.Zero;

        private Color _borderColor;
        private float _borderWidth;

        private readonly double _scorePrepareDuration = 1.5;
        private readonly double _specialEndDuration = 1.5;

        private enum OngoingAnimation {

            None = 0,
            ScorePrepare = 1,
            SpecialEnd = 2

        }

    }
}
