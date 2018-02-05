using System;
using System.Drawing;
using System.IO;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Contributed.Scores;
using OpenMLTD.MilliSim.Contributed.Scores.Animation;
using OpenMLTD.MilliSim.Contributed.Scores.Extensions;
using OpenMLTD.MilliSim.Contributed.Scores.Runtime;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Advanced;
using OpenMLTD.MilliSim.Graphics.Extensions;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Gaming {
    public class SlideMotion : Visual {

        public SlideMotion([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        protected override void OnDraw(GameTime gameTime) {
            base.OnDraw(gameTime);

            var config = ConfigurationStore.Get<SlideMotionConfig>();

            var motionIcon = config.Data.Icon;
            if (motionIcon == SlideMotionConfig.SlideMotionIcon.None) {
                return;
            }

            if (_score == null) {
                return;
            }

            var notes = _score.Notes;
            var theaterDays = Game.ToBaseGame();

            var syncTimer = theaterDays.FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                return;
            }

            var tapPoints = theaterDays.FindSingleElement<TapPoints>();
            if (tapPoints == null) {
                throw new InvalidOperationException();
            }

            var notesLayer = theaterDays.FindSingleElement<NotesLayer>();
            if (notesLayer == null) {
                throw new InvalidOperationException();
            }

            var scalingResponder = theaterDays.FindSingleElement<MltdStageScalingResponder>();
            if (scalingResponder == null) {
                throw new InvalidOperationException();
            }

            var now = (float)syncTimer.CurrentTime.TotalSeconds;

            var tapPointsConfig = ConfigurationStore.Get<TapPointsConfig>();
            var notesLayerConfig = ConfigurationStore.Get<NotesLayerConfig>();
            var tapPointsLayout = tapPointsConfig.Data.Layout;
            var notesLayerLayout = notesLayerConfig.Data.Layout;
            var clientSize = theaterDays.GraphicsDevice.Viewport;

            var animationCalculator = notesLayer.AnimationCalculator;

            var commonNoteMetrics = new NoteMetrics {
                StartRadius = scalingResponder.ScaleResults.Note.Start,
                EndRadius = scalingResponder.ScaleResults.Note.End
            };

            var animationMetrics = new NoteAnimationMetrics {
                GlobalSpeedScale = notesLayer.GlobalSpeedScale,
                Width = clientSize.Width,
                Height = clientSize.Height,
                Top = notesLayerLayout.Y.IsPercentage ? notesLayerLayout.Y.Value * clientSize.Height : notesLayerLayout.Y.Value,
                Bottom = tapPointsLayout.Y.IsPercentage ? tapPointsLayout.Y.Value * clientSize.Height : tapPointsLayout.Y.Value,
                NoteStartXRatios = tapPoints.StartXRatios,
                NoteEndXRatios = tapPoints.EndXRatios,
                TrackCount = tapPoints.EndXRatios.Length
            };

            var spriteBatch = theaterDays.SpriteBatch;

            spriteBatch.Begin();

            foreach (var note in notes) {
                if (!note.IsSlide() || !note.HasNextSlide()) {
                    continue;
                }

                var thisStatus = NoteAnimationHelper.GetOnStageStatusOf(note, now, animationMetrics);
                if (thisStatus < OnStageStatus.Passed) {
                    continue;
                }

                var nextStatus = NoteAnimationHelper.GetOnStageStatusOf(note.NextSlide, now, animationMetrics);
                if (nextStatus >= OnStageStatus.Passed) {
                    continue;
                }

                var x = animationCalculator.GetNoteX(note, now, commonNoteMetrics, animationMetrics);
                var y = animationCalculator.GetNoteY(note, now, commonNoteMetrics, animationMetrics);

                Vector2 imageSize;
                switch (motionIcon) {
                    case SlideMotionConfig.SlideMotionIcon.None:
                        // Not possible.
                        throw new InvalidOperationException();
                    case SlideMotionConfig.SlideMotionIcon.TapPoint:
                        if (_tapPointImage != null) {
                            imageSize = scalingResponder.ScaleResults.TapPoint.Start;
                            spriteBatch.Draw(_tapPointImage, RectHelper.RoundToRectangle(x - imageSize.X / 2, y - imageSize.Y / 2, imageSize.X, imageSize.Y));
                        }
                        break;
                    case SlideMotionConfig.SlideMotionIcon.SlideStart:
                    case SlideMotionConfig.SlideMotionIcon.SlideMiddle:
                    case SlideMotionConfig.SlideMotionIcon.SlideEnd:
                        var isStart = motionIcon == SlideMotionConfig.SlideMotionIcon.SlideStart;
                        var isEnd = motionIcon == SlideMotionConfig.SlideMotionIcon.SlideEnd;
                        if (_noteImages?[0] != null) {
                            var (imageIndex, _) = NotesLayer.GetImageIndex(NoteType.Slide, NoteSize.Small, FlickDirection.None, false, false, isStart, isEnd);
                            imageSize = scalingResponder.ScaleResults.Note.End;
                            spriteBatch.Draw(_noteImages[0], imageIndex, RectHelper.RoundToRectangle(x - imageSize.X / 2, y - imageSize.Y / 2, imageSize.X, imageSize.Y));
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            spriteBatch.End();
        }

        protected override void OnLoadContents() {
            base.OnLoadContents();

            var notesLayerConfig = ConfigurationStore.Get<NotesLayerConfig>();
            var theaterDays = Game.ToBaseGame();
            var debugOverlay = theaterDays.FindSingleElement<DebugOverlay>();

            var gamingArea = theaterDays.FindSingleElement<MltdStage>();
            if (gamingArea == null) {
                throw new InvalidOperationException();
            }

            if (notesLayerConfig.Data.Images.Notes == null || notesLayerConfig.Data.Images.Notes.Length == 0) {
                if (debugOverlay != null) {
                    debugOverlay.AddLine("WARNING: default notes image strip is not specified.");
                }
            } else {
                _noteImages = new SpriteSheet1D[notesLayerConfig.Data.Images.Notes.Length];

                for (var i = 0; i < notesLayerConfig.Data.Images.Notes.Length; ++i) {
                    var notesImageEntry = notesLayerConfig.Data.Images.Notes[i];
                    if (notesImageEntry == null) {
                        continue;
                    }

                    if (notesImageEntry.File == null || !File.Exists(notesImageEntry.File)) {
                        if (i == 0) {
                            if (debugOverlay != null) {
                                debugOverlay.AddLine($"WARNING: default notes image strip <{notesImageEntry.File ?? string.Empty}> is not found.");
                            }
                        } else {
                            if (debugOverlay != null) {
                                debugOverlay.AddLine($"WARNING: notes image strip <{notesImageEntry.File ?? string.Empty}> is not found, falling back to default.");
                            }
                        }
                        continue;
                    }

                    var imageStrip = ContentHelper.LoadSpriteSheet1D(theaterDays.GraphicsDevice, notesImageEntry.File, notesImageEntry.Count, (SpriteSheetOrientation)notesImageEntry.Orientation);
                    _noteImages[i] = imageStrip;
                }
            }

            var tapPointsConfig = ConfigurationStore.Get<TapPointsConfig>();
            _tapPointImage = ContentHelper.LoadTexture(theaterDays.GraphicsDevice, tapPointsConfig.Data.Images.TapPoint.FileName);
        }

        protected override void OnUnloadContents() {
            base.OnUnloadContents();

            if (_noteImages != null) {
                foreach (var noteImage in _noteImages) {
                    noteImage?.Dispose();
                }
            }
            _noteImages = null;

            _tapPointImage?.Dispose();
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            var scoreLoader = Game.ToBaseGame().FindSingleElement<ScoreLoader>();
            _score = scoreLoader?.RuntimeScore;
        }

        [CanBeNull, ItemCanBeNull]
        private SpriteSheet1D[] _noteImages;
        [CanBeNull]
        private Texture2D _tapPointImage;

        private RuntimeScore _score;

    }
}
