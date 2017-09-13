using System;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Runtime.Extensions;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Graphics.Rendering;
using OpenMLTD.MilliSim.Graphics.Rendering.Direct3D;
using OpenMLTD.MilliSim.Graphics.Rendering.Direct3D.Effects;
using OpenMLTD.MilliSim.Theater.Extensions;
using OpenMLTD.MilliSim.Theater.Intenal;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Color = System.Drawing.Color;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public class RibbonsLayer : VisualElement {

        public RibbonsLayer(GameBase game)
            : base(game) {
        }

        protected override void OnDraw(GameTime gameTime, RenderContext context) {
            base.OnDraw(gameTime, context);

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

            var settings = Program.Settings;
            var now = syncTimer.CurrentTime.TotalSeconds;
            var tapPointsLayout = settings.UI.TapPoints.Layout;
            var notesLayerLayout = settings.UI.NotesLayer.Layout;
            var scaledNoteSize = settings.Scaling.Note;
            var clientSize = context.ClientSize;

            var traceCalculator = NotesLayer.TraceCalculator;

            var animationMetrics = new NoteAnimationMetrics {
                ClientSize = clientSize,
                GlobalSpeedScale = notesLayer.GlobalSpeedScale,
                Top = notesLayerLayout.Y * clientSize.Height,
                Bottom = tapPointsLayout.Y * clientSize.Height,
                NoteStartXRatios = tapPoints.IncomingXRatios,
                NoteEndXRatios = tapPoints.TapPointXRatios,
                TrackCount = tapPoints.TapPointXRatios.Length
            };

            var noteMetrics = new NoteMetrics {
                StartRadius = scaledNoteSize,
                EndRadius = scaledNoteSize,
                GlobalSpeedScale = notesLayer.GlobalSpeedScale
            };

            context.Begin2D();

            foreach (var note in notes) {
                if (note.HasNextHold()) {
                    var thisStatus = NoteAnimationHelper.GetOnStageStatusOf(note, now, noteMetrics);

                    var nextHold = note.NextHold;
                    var nextHoldStatus = NoteAnimationHelper.GetOnStageStatusOf(nextHold, now, noteMetrics);

                    // Not even entered, or both left.
                    if (thisStatus == OnStageStatus.Incoming || ((int)thisStatus * (int)nextHoldStatus > 0)) {
                        continue;
                    }

                    var thisX = traceCalculator.GetNoteX(note, now, noteMetrics, animationMetrics);
                    var thisY = traceCalculator.GetNoteY(note, now, noteMetrics, animationMetrics);
                    var nextX = traceCalculator.GetNextNoteX(note, nextHold, now, noteMetrics, animationMetrics);
                    var nextY = traceCalculator.GetNoteY(nextHold, now, noteMetrics, animationMetrics);
                    context.DrawLine(_simpleRibbonPen, thisX, thisY, nextX, nextY);
                }

                if (note.HasNextSlide()) {
                    var thisStatus = NoteAnimationHelper.GetOnStageStatusOf(note, now, noteMetrics);

                    var nextSlide = note.NextSlide;
                    var nextSlideStatus = NoteAnimationHelper.GetOnStageStatusOf(nextSlide, now, noteMetrics);

                    // Not even entered, or both left.
                    if (thisStatus == OnStageStatus.Incoming || ((int)thisStatus * (int)nextSlideStatus > 0)) {
                        continue;
                    }
                    var thisX = traceCalculator.GetNoteX(note, now, noteMetrics, animationMetrics);
                    var thisY = traceCalculator.GetNoteY(note, now, noteMetrics, animationMetrics);
                    var nextX = traceCalculator.GetNextNoteX(note, nextSlide, now, noteMetrics, animationMetrics);
                    var nextY = traceCalculator.GetNoteY(nextSlide, now, noteMetrics, animationMetrics);
                    context.DrawLine(_simpleRibbonPen, thisX, thisY, nextX, nextY);
                }
            }

            context.End2D();
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);

            _simpleRibbonPen = new D2DPen(context, Color.White, 20);
        }

        protected override void OnLostContext(RenderContext context) {
            base.OnLostContext(context);

            _simpleRibbonPen.Dispose();
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            var scoreLoader = Game.AsTheaterDays().FindSingleElement<ScoreLoader>();
            _score = scoreLoader?.RuntimeScore;
        }

        private D2DPen _simpleRibbonPen;

        private RuntimeScore _score;

    }
}
