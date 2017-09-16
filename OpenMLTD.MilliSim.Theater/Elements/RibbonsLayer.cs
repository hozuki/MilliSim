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
using OpenMLTD.MilliSim.Theater.Internal;
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

            var gamingArea = theaterDays.FindSingleElement<GamingArea>();
            if (gamingArea == null) {
                throw new InvalidOperationException();
            }

            var settings = Program.Settings;
            var now = syncTimer.CurrentTime.TotalSeconds;
            var tapPointsLayout = settings.UI.TapPoints.Layout;
            var notesLayerLayout = settings.UI.NotesLayer.Layout;
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

            var commonNoteMetrics = new NoteMetrics {
                StartRadius = gamingArea.ScaleResults.Note.Start,
                EndRadius = gamingArea.ScaleResults.Note.End,
                GlobalSpeedScale = notesLayer.GlobalSpeedScale
            };

            context.Begin2D();

            foreach (var note in notes) {
                OnStageStatus thisStatus, nextStatus;
                RuntimeNote nextNote;
                RibbonParameters ribbonParams;

                if (note.HasNextHold()) {
                    thisStatus = NoteAnimationHelper.GetOnStageStatusOf(note, now, commonNoteMetrics);

                    nextNote = note.NextHold;
                    nextStatus = NoteAnimationHelper.GetOnStageStatusOf(nextNote, now, commonNoteMetrics);

                    // Not even entered, or both left.
                    if (thisStatus == OnStageStatus.Incoming || ((int)thisStatus * (int)nextStatus > 0)) {
                        continue;
                    }

                    ribbonParams = traceCalculator.GetHoldRibbonParameters(note, nextNote, now, commonNoteMetrics, animationMetrics);
                    if (ribbonParams.IsLine) {
                        context.DrawLine(_simpleRibbonPen, ribbonParams.X1, ribbonParams.Y1, ribbonParams.X2, ribbonParams.Y2);
                    } else {
                        context.DrawBezier(_simpleRibbonPen, ribbonParams.X1, ribbonParams.Y1, ribbonParams.ControlX1, ribbonParams.ControlY1, ribbonParams.ControlX2, ribbonParams.ControlY2, ribbonParams.X2, ribbonParams.Y2);
                    }
                }

                if (note.HasNextSlide()) {
                    thisStatus = NoteAnimationHelper.GetOnStageStatusOf(note, now, commonNoteMetrics);

                    nextNote = note.NextSlide;
                    nextStatus = NoteAnimationHelper.GetOnStageStatusOf(nextNote, now, commonNoteMetrics);

                    // Not even entered, or both left.
                    if (thisStatus == OnStageStatus.Incoming || ((int)thisStatus * (int)nextStatus > 0)) {
                        continue;
                    }

                    ribbonParams = traceCalculator.GetSlideRibbonParameters(note, nextNote, now, commonNoteMetrics, animationMetrics);
                    if (ribbonParams.IsLine) {
                        context.DrawLine(_simpleRibbonPen, ribbonParams.X1, ribbonParams.Y1, ribbonParams.X2, ribbonParams.Y2);
                    } else {
                        context.DrawBezier(_simpleRibbonPen, ribbonParams.X1, ribbonParams.Y1, ribbonParams.ControlX1, ribbonParams.ControlY1, ribbonParams.ControlX2, ribbonParams.ControlY2, ribbonParams.X2, ribbonParams.Y2);
                    }
                }
            }

            context.End2D();
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);

            var gamingArea = Game.AsTheaterDays().FindSingleElement<GamingArea>();
            if (gamingArea == null) {
                throw new InvalidOperationException();
            }

            _simpleRibbonPen = new D2DPen(context, Color.White, gamingArea.ScaleResults.Ribbon.Width);
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
