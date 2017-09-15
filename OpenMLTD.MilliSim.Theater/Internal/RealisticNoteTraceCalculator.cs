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
            if (onStage == OnStageStatus.Passed) {
                if (note.HasNextSlide()) {
                    var destXRatio = endLeftMarginRatio + (endRightMarginRatio - endLeftMarginRatio) * (note.NextSlide.EndX / (trackCount - 1));
                    var nextPerc = (float)(now - note.HitTime) / (float)(note.NextSlide.HitTime - note.HitTime);
                    xRatio = MathHelper.Lerp(endXRatio, destXRatio, nextPerc);
                } else {
                    xRatio = endXRatio;
                }
            } else {
                var startLeftMarginRatio = animationMetrics.NoteStartXRatios[0];
                var startRightMarginRatio = animationMetrics.NoteStartXRatios[trackCount - 1];
                var whichStartToTake = note.IsSlide() ? note.EndX : note.StartX;
                var startXRatio = startLeftMarginRatio + (startRightMarginRatio - startLeftMarginRatio) * (whichStartToTake / (trackCount - 1));

                var timePoints = NoteAnimationHelper.CalculateNoteTimePoints(note, noteMetrics);
                var perc = (now - timePoints.Enter) / timePoints.Duration;

                xRatio = (float)MathHelper.Lerp(startXRatio, endXRatio, perc);
            }

            return animationMetrics.ClientSize.Width * xRatio;
        }

        public float GetNextNoteX(RuntimeNote thisNote, RuntimeNote nextNote, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            if (thisNote == null) {
                return GetNoteX(nextNote, now, noteMetrics, animationMetrics);
            }

            var nextOnStage = NoteAnimationHelper.GetOnStageStatusOf(nextNote, now, noteMetrics);
            if (nextOnStage >= OnStageStatus.Visible) {
                return GetNoteX(nextNote, now, noteMetrics, animationMetrics);
            } else {
                var trackCount = animationMetrics.TrackCount;
                var startLeftMarginRatio = animationMetrics.NoteStartXRatios[0];
                var startRightMarginRatio = animationMetrics.NoteStartXRatios[trackCount - 1];

                float xRatio;
                if (nextNote.IsSlide()) {
                    var thisXRatio = startLeftMarginRatio + (startRightMarginRatio - startLeftMarginRatio) * (thisNote.EndX / (trackCount - 1));
                    var nextXRatio = startLeftMarginRatio + (startRightMarginRatio - startLeftMarginRatio) * (nextNote.EndX / (trackCount - 1));

                    var thisTimePoints = NoteAnimationHelper.CalculateNoteTimePoints(thisNote, noteMetrics);
                    var nextTimePoints = NoteAnimationHelper.CalculateNoteTimePoints(nextNote, noteMetrics);

                    var perc = (float)(now - thisTimePoints.Enter) / (float)(nextTimePoints.Enter - thisTimePoints.Enter);
                    xRatio = MathHelper.Lerp(thisXRatio, nextXRatio, perc);

                } else {
                    xRatio = startLeftMarginRatio + (startRightMarginRatio - startLeftMarginRatio) * (nextNote.StartX / (trackCount - 1));
                }

                return animationMetrics.ClientSize.Width * xRatio;
            }
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
                    y = MathHelper.Lerp(animationMetrics.Top, animationMetrics.Bottom, (float)(now - timePoints.Enter) / (float)timePoints.Duration);
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

        private static float WtfTransform(float f) {
            return f / (2 - f);
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

    }
}
