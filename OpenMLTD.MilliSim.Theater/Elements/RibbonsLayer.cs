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
            var currentSecond = syncTimer.CurrentTime.TotalSeconds;
            var speedScale = notesLayer.SpeedScale;
            var startXRatios = tapPoints.IncomingXRatios;
            var endXRatios = tapPoints.TapPointXRatios;
            var tapPointsLayout = settings.UI.TapPoints.Layout;
            var notesLayerLayout = settings.UI.NotesLayer.Layout;
            var clientSize = context.ClientSize;

            var renderMode = notesLayer.RenderMode;

            var topY = notesLayerLayout.Y * clientSize.Height;
            var bottomY = tapPointsLayout.Y * clientSize.Height;

            context.Begin2D();

            foreach (var note in notes) {
                OnStageStatus? thisStatus = null;
                (float X, float Y, bool Visible)? thisLocation = null;

                if (note.HasNextHold()) {
                    if (thisStatus == null) {
                        thisStatus = RuntimeNoteCalculator.GetOnStageStatusOf(note, currentSecond, speedScale);
                    }

                    var nextHold = note.NextHold;
                    var nextHoldStatus = RuntimeNoteCalculator.GetOnStageStatusOf(nextHold, currentSecond, speedScale);

                    // Not even entered, or both left.
                    if (thisStatus.Value == OnStageStatus.Incoming || ((int)thisStatus.Value * (int)nextHoldStatus > 0)) {
                        continue;
                    }

                    if (thisLocation == null) {
                        thisLocation = RuntimeNoteCalculator.CalculateRibbonLocation(note, currentSecond, startXRatios, endXRatios, clientSize, topY, bottomY, speedScale, renderMode, false);
                    }
                    var nextHoldLocaion = RuntimeNoteCalculator.CalculateNoteLocation(nextHold, currentSecond, startXRatios, endXRatios, clientSize, topY, bottomY, speedScale, renderMode, false);
                    context.DrawLine(_simpleRibbonPen, thisLocation.Value.X, thisLocation.Value.Y, nextHoldLocaion.X, nextHoldLocaion.Y);
                }

                if (note.HasNextSlide()) {
                    if (thisStatus == null) {
                        thisStatus = RuntimeNoteCalculator.GetOnStageStatusOf(note, currentSecond, speedScale);
                    }

                    var nextSlide = note.NextSlide;
                    var nextSlideStatus = RuntimeNoteCalculator.GetOnStageStatusOf(nextSlide, currentSecond, speedScale);

                    // Not even entered, or both left.
                    if (thisStatus.Value == OnStageStatus.Incoming || ((int)thisStatus.Value * (int)nextSlideStatus > 0)) {
                        continue;
                    }

                    if (thisLocation == null) {
                        thisLocation = RuntimeNoteCalculator.CalculateRibbonLocation(note, currentSecond, startXRatios, endXRatios, clientSize, topY, bottomY, speedScale, renderMode, false);
                    }
                    var nextSlideLocaion = RuntimeNoteCalculator.CalculateNoteLocation(nextSlide, currentSecond, startXRatios, endXRatios, clientSize, topY, bottomY, speedScale, renderMode, false);
                    context.DrawLine(_simpleRibbonPen, thisLocation.Value.X, thisLocation.Value.Y, nextSlideLocaion.X, nextSlideLocaion.Y);
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
