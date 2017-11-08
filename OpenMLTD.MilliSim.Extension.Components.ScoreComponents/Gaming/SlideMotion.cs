using System;
using System.Drawing;
using System.IO;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Core.Entities.Extensions;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.GameAbstraction.Extensions;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D.Advanced;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Theater.Animation;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Gaming {
    public class SlideMotion : Visual {

        public SlideMotion([NotNull] IVisualContainer parent)
            : base(parent) {
        }

        protected override void OnDraw(GameTime gameTime, RenderContext context) {
            base.OnDraw(gameTime, context);

            var config = ConfigurationStore.Get<SlideMotionConfig>();

            var motionIcon = config.Data.Icon;
            if (motionIcon == SlideMotionConfig.SlideMotionIcon.None) {
                return;
            }

            if (_score == null) {
                return;
            }

            var notes = _score.Notes;
            var theaterDays = Game.AsTheaterDays();

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

            var now = syncTimer.CurrentTime.TotalSeconds;

            var tapPointsConfig = ConfigurationStore.Get<TapPointsConfig>();
            var notesLayerConfig = ConfigurationStore.Get<NotesLayerConfig>();
            var tapPointsLayout = tapPointsConfig.Data.Layout;
            var notesLayerLayout = notesLayerConfig.Data.Layout;
            var clientSize = context.ClientSize;

            var traceCalculator = notesLayer.TraceCalculator;

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

            context.Begin2D();

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

                var x = traceCalculator.GetNoteX(note, now, commonNoteMetrics, animationMetrics);
                var y = traceCalculator.GetNoteY(note, now, commonNoteMetrics, animationMetrics);

                SizeF imageSize;
                switch (motionIcon) {
                    case SlideMotionConfig.SlideMotionIcon.None:
                        // Not possible.
                        throw new InvalidOperationException();
                    case SlideMotionConfig.SlideMotionIcon.TapPoint:
                        if (_tapPointImage != null) {
                            imageSize = scalingResponder.ScaleResults.TapPoint.Start;
                            context.DrawBitmap(_tapPointImage, x - imageSize.Width / 2, y - imageSize.Height / 2, imageSize.Width, imageSize.Height);
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
                            context.DrawImageStripUnit(_noteImages[0], imageIndex, x - imageSize.Width / 2, y - imageSize.Height / 2, imageSize.Width, imageSize.Height);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            context.End2D();
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);

            var notesLayerConfig = ConfigurationStore.Get<NotesLayerConfig>();
            var theaterDays = Game.AsTheaterDays();
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
                _noteImages = new D2DImageStrip[notesLayerConfig.Data.Images.Notes.Length];

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

                    var imageStrip = Direct2DHelper.LoadImageStrip(context, notesImageEntry.File, notesImageEntry.Count, (ImageStripOrientation)notesImageEntry.Orientation);
                    _noteImages[i] = imageStrip;
                }
            }

            var tapPointsConfig = ConfigurationStore.Get<TapPointsConfig>();
            _tapPointImage = Direct2DHelper.LoadBitmap(context, tapPointsConfig.Data.Images.TapPoint.FileName);
        }

        protected override void OnLostContext(RenderContext context) {
            base.OnLostContext(context);

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

            var scoreLoader = Game.AsTheaterDays().FindSingleElement<ScoreLoader>();
            _score = scoreLoader?.RuntimeScore;
        }

        [CanBeNull, ItemCanBeNull]
        private D2DImageStrip[] _noteImages;
        [CanBeNull]
        private D2DBitmap _tapPointImage;

        private RuntimeScore _score;

    }
}
