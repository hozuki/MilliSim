using System;
using System.Drawing;
using System.IO;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Core.Entities.Extensions;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D.Advanced;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Theater.Animation;
using OpenMLTD.MilliSim.Theater.Configuration.Primitives;
using OpenMLTD.MilliSim.Theater.Elements.Logical;
using OpenMLTD.MilliSim.Theater.Elements.Visual.Overlays;
using OpenMLTD.MilliSim.Theater.Extensions;

namespace OpenMLTD.MilliSim.Theater.Elements.Visual.Gaming {
    public class SlideMotion : Graphics.Visual {

        public SlideMotion([NotNull] IVisualContainer parent)
            : base(parent) {
        }

        protected override void OnDraw(GameTime gameTime, RenderContext context) {
            base.OnDraw(gameTime, context);

            var settings = Program.Settings;

            var motionIcon = settings.Style.SlideMotionIcon;
            if (motionIcon == SlideMotionIcon.None) {
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

            var gamingArea = theaterDays.FindSingleElement<GamingArea>();
            if (gamingArea == null) {
                throw new InvalidOperationException();
            }

            var now = syncTimer.CurrentTime.TotalSeconds;
            var tapPointsLayout = settings.UI.TapPoints.Layout;
            var notesLayerLayout = settings.UI.NotesLayer.Layout;
            var clientSize = context.ClientSize;

            var traceCalculator = notesLayer.TraceCalculator;

            var commonNoteMetrics = new NoteMetrics {
                StartRadius = gamingArea.ScaleResults.Note.Start,
                EndRadius = gamingArea.ScaleResults.Note.End
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
                    case SlideMotionIcon.None:
                        // Not possible.
                        throw new InvalidOperationException();
                    case SlideMotionIcon.Tap:
                        if (_tapPointImage != null) {
                            imageSize = gamingArea.ScaleResults.TapPoint.Start;
                            context.DrawBitmap(_tapPointImage, x - imageSize.Width / 2, y - imageSize.Height / 2, imageSize.Width, imageSize.Height);
                        }
                        break;
                    case SlideMotionIcon.Slide:
                        if (_noteImages?[0] != null) {
                            var (imageIndex, _) = NotesLayer.GetImageIndex(NoteType.Slide, NoteSize.Small, FlickDirection.None, false, false, false, false);
                            imageSize = gamingArea.ScaleResults.Note.End;
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

            var settings = Program.Settings;
            var theaterDays = Game.AsTheaterDays();
            var debugOverlay = theaterDays.FindSingleElement<DebugOverlay>();

            var gamingArea = theaterDays.FindSingleElement<GamingArea>();
            if (gamingArea == null) {
                throw new InvalidOperationException();
            }

            if (settings.Images.Notes == null || settings.Images.Notes.Length == 0) {
                if (debugOverlay != null) {
                    debugOverlay.AddLine("WARNING: default notes image strip is not specified.");
                }
            } else {
                _noteImages = new D2DImageStrip[settings.Images.Notes.Length];

                for (var i = 0; i < settings.Images.Notes.Length; ++i) {
                    var notesImageEntry = settings.Images.Notes[i];
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

                    var imageStrip = Direct2DHelper.LoadImageStrip(context, notesImageEntry.File, notesImageEntry.Count, notesImageEntry.Orientation);
                    _noteImages[i] = imageStrip;
                }
            }

            _tapPointImage = Direct2DHelper.LoadBitmap(context, settings.Images.TapPoint.FileName);
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
