using System;
using System.Drawing;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Runtime.Extensions;

namespace OpenMLTD.MilliSim.Theater.Internal {
    internal sealed class RealisticNoteTraceCalculator : INoteTraceCalculator {

        public SizeF GetNoteRadius(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var timePoints = NoteAnimationHelper.CalculateNoteTimePoints(note, noteMetrics);
            var passed = now - timePoints.Enter;
            //var remaining = timePoints.Leave - now;
            var perc = passed / timePoints.Duration;
            var w = (float)MathHelper.Lerp(noteMetrics.StartRadius.Width, noteMetrics.EndRadius.Width, perc);
            var h = (float)MathHelper.Lerp(noteMetrics.StartRadius.Height, noteMetrics.EndRadius.Height, perc);
            return new SizeF(w, h);
        }

        public float GetNoteX(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var trackCount = animationMetrics.TrackCount;
            var endLeftMarginRatio = animationMetrics.NoteEndXRatios[0];
            var endRightMarginRatio = animationMetrics.NoteEndXRatios[trackCount - 1];

            var endXRatio = endLeftMarginRatio + (endRightMarginRatio - endLeftMarginRatio) * (note.EndX / (trackCount - 1));

            var onStage = NoteAnimationHelper.GetOnStageStatusOf(note, now, noteMetrics);
            float xRatio;
            switch (onStage) {
                case OnStageStatus.Incoming:
                    if (note.IsHoldEnd()) {
                        xRatio = GetIncomingNoteXRatio(note.PrevHold, note, now, noteMetrics, animationMetrics);
                    } else if (note.IsSlideEnd()) {
                        xRatio = GetIncomingNoteXRatio(note.PrevSlide, note, now, noteMetrics, animationMetrics);
                    } else {
                        xRatio = endXRatio;
                    }
                    break;
                case OnStageStatus.Passed:
                    if (note.HasNextSlide()) {
                        var destXRatio = endLeftMarginRatio + (endRightMarginRatio - endLeftMarginRatio) * (note.NextSlide.EndX / (trackCount - 1));
                        var nextPerc = (float)(now - note.HitTime) / (float)(note.NextSlide.HitTime - note.HitTime);
                        xRatio = MathHelper.Lerp(endXRatio, destXRatio, nextPerc);
                    } else {
                        xRatio = endXRatio;
                    }
                    break;
                default:
                    var startLeftMarginRatio = animationMetrics.NoteStartXRatios[0];
                    var startRightMarginRatio = animationMetrics.NoteStartXRatios[trackCount - 1];

                    float whichStartToTake;
                    if (note.IsSlide()) {
                        whichStartToTake = note.EndX;
                    } else {
                        whichStartToTake = note.StartX < 0 ? note.StartX * 0.5f : note.StartX;
                    }

                    var startXRatio = startLeftMarginRatio + (startRightMarginRatio - startLeftMarginRatio) * (whichStartToTake / (trackCount - 1));

                    var timePoints = NoteAnimationHelper.CalculateNoteTimePoints(note, noteMetrics);
                    var perc = (now - timePoints.Enter) / timePoints.Duration;

                    xRatio = (float)MathHelper.Lerp(startXRatio, endXRatio, perc);
                    break;
            }

            return animationMetrics.ClientSize.Width * xRatio;
        }

        public float GetNoteY(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var timePoints = NoteAnimationHelper.CalculateNoteTimePoints(note, noteMetrics);
            var onStageStatus = NoteAnimationHelper.GetOnStageStatusOf(note, now, timePoints);

            float y;
            switch (onStageStatus) {
                case OnStageStatus.Incoming:
                    y = animationMetrics.Top;
                    break;
                case OnStageStatus.Visible:
                    y = GetNoteOnStageY(note, now, timePoints, animationMetrics);
                    break;
                case OnStageStatus.Passed:
                    y = animationMetrics.Bottom;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return y;
        }

        public SizeF GetSpecialNoteRadius(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            return GetNoteRadius(note, now, noteMetrics, animationMetrics);
        }

        public float GetSpecialNoteX(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var leftRatio = animationMetrics.NoteEndXRatios[0];
            var rightRatio = animationMetrics.NoteEndXRatios[animationMetrics.TrackCount - 1];
            var xRatio = (leftRatio + rightRatio) / 2;
            return animationMetrics.ClientSize.Width * xRatio;
        }

        public float GetSpecialNoteY(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            return GetNoteY(note, now, noteMetrics, animationMetrics);
        }

        public (float X1, float Y1, float ControlX1, float ControlY1, float ControlX2, float ControlY2, float X2, float Y2) GetRibbonLocations(RuntimeNote thisNote, RuntimeNote nextNote, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            throw new NotImplementedException();
        }

        private static float GetIncomingNoteXRatio(RuntimeNote prevNote, RuntimeNote thisNote, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var trackCount = animationMetrics.TrackCount;
            var startLeftMarginRatio = animationMetrics.NoteStartXRatios[0];
            var startRightMarginRatio = animationMetrics.NoteStartXRatios[trackCount - 1];

            float xRatio;
            if (thisNote.IsSlide()) {
                var thisXRatio = startLeftMarginRatio + (startRightMarginRatio - startLeftMarginRatio) * (prevNote.EndX / (trackCount - 1));
                var nextXRatio = startLeftMarginRatio + (startRightMarginRatio - startLeftMarginRatio) * (thisNote.EndX / (trackCount - 1));

                var thisTimePoints = NoteAnimationHelper.CalculateNoteTimePoints(prevNote, noteMetrics);
                var nextTimePoints = NoteAnimationHelper.CalculateNoteTimePoints(thisNote, noteMetrics);

                var perc = (float)(now - thisTimePoints.Enter) / (float)(nextTimePoints.Enter - thisTimePoints.Enter);
                xRatio = MathHelper.Lerp(thisXRatio, nextXRatio, perc);

            } else {
                var nextStartX = thisNote.StartX < 0 ? thisNote.StartX * 0.5f : thisNote.StartX;
                xRatio = startLeftMarginRatio + (startRightMarginRatio - startLeftMarginRatio) * (nextStartX / (trackCount - 1));
            }

            return xRatio;
        }

        private static double WtfTransform(double f) {
            return f / (2 - f);
        }

        private static double GetTransformedTime(RuntimeNote note, double now, NoteTimePoints timePoints, bool clampIncoming = false, bool clampPassed = false) {
            var timeRemaining = note.HitTime - now;
            var timeRemainingInWindow = timeRemaining / timePoints.Duration;
            if (clampIncoming && timeRemaining > timePoints.Duration) {
                timeRemainingInWindow = 1f;
            }
            if (clampPassed && timeRemaining < 0f) {
                timeRemainingInWindow = 0f;
            }
            return WtfTransform(timeRemainingInWindow);
        }

        private static float GetNoteTransformedY(double transformedTime) {
            return (float)transformedTime + 1.2f * (float)transformedTime * (1f - (float)transformedTime);
        }

        private static float GetNoteTransformedX(double transformedTime) {
            return (float)transformedTime;
        }

        private static float GetNoteOnStageY(double transformedTime, NoteAnimationMetrics animationMetrics) {
            var transformedY = GetNoteTransformedY(transformedTime);
            var y = animationMetrics.Bottom + (animationMetrics.Top - animationMetrics.Bottom) * transformedY;
            return y;
        }

        private static float GetNoteOnStageY(RuntimeNote note, double now, NoteTimePoints timePoints, NoteAnimationMetrics animationMetrics) {
            var transformedTime = GetTransformedTime(note, now, timePoints, true, true);
            return GetNoteOnStageY(transformedTime, animationMetrics);
        }

        private static (float ControlX1, float ControlX2) GetBezierFromQuadratic(float x1, float xmid, float x2) {
            var xcontrol = xmid * 2f - (x1 + x2) * 0.5f;
            var cx1 = (x1 + xcontrol * 2f) / 3f;
            var cx2 = (x2 + xcontrol * 2f) / 3f;
            return (cx1, cx2);
        }

    }
}
