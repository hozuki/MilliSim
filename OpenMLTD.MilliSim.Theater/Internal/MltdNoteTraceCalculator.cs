using System;
using System.Drawing;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Runtime.Extensions;

namespace OpenMLTD.MilliSim.Theater.Internal {
    /// <inheritdoc cref="INoteTraceCalculator"/>
    /// <summary>
    /// This class calculates note traces in MLTD style.
    /// </summary>
    internal sealed class MltdNoteTraceCalculator : INoteTraceCalculator {

        public SizeF GetNoteRadius(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var timePoints = NoteAnimationHelper.CalculateNoteTimePoints(note, noteMetrics);
            var passed = now - timePoints.Enter;
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
                    if (note.HasPrevHold()) {
                        xRatio = GetIncomingNoteXRatio(note.PrevHold, note, now, noteMetrics, animationMetrics);
                    } else if (note.HasPrevSlide()) {
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

        public RibbonParameters GetHoldRibbonParameters(RuntimeNote startNote, RuntimeNote endNote, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var tp1 = NoteAnimationHelper.CalculateNoteTimePoints(startNote, noteMetrics);
            var tp2 = NoteAnimationHelper.CalculateNoteTimePoints(endNote, noteMetrics);

            var t1 = GetTransformedTime(startNote, now, tp1);
            var t2 = GetTransformedTime(endNote, now, tp2);
            var tperc = startNote.IsHold() ? 0.5 : 0.4;
            var tm = MathHelper.Lerp(t1, t2, tperc);

            var x1 = GetNoteX(startNote, now, noteMetrics, animationMetrics);
            var y1 = GetNoteOnStageY(t1, animationMetrics);
            var x2 = GetNoteX(endNote, now, noteMetrics, animationMetrics);
            var y2 = GetNoteOnStageY(t2, animationMetrics);

            // CGSS-like
            //var xm = GetNoteOnStageX(endNote.StartX, endNote.EndX, tm, animationMetrics);
            // Naive guess
            //var xm = (x1 + x2) / 2;
            var xm = MathHelper.Lerp(x1, x2, 0.5f);
            if (startNote.IsSlide()) {
                if (endNote.EndX < startNote.EndX) {
                    xm -= animationMetrics.ClientSize.Width * 0.02f * (float)((tp2.Leave - now) / (tp2.Leave - tp1.Enter));
                } else if (endNote.EndX > startNote.EndX) {
                    xm += animationMetrics.ClientSize.Width * 0.02f * (float)((tp2.Leave - now) / (tp2.Leave - tp1.Enter));
                }
            }
            var ym = GetNoteOnStageY(tm, animationMetrics);
            var (cx1, cx2) = GetBezierFromQuadratic(x1, xm, x2);
            var (cy1, cy2) = GetBezierFromQuadratic(y1, ym, y2);
            return new RibbonParameters(x1, y1, cx1, cy1, cx2, cy2, x2, y2);
        }

        public RibbonParameters GetSlideRibbonParameters(RuntimeNote startNote, RuntimeNote endNote, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var tp1 = NoteAnimationHelper.CalculateNoteTimePoints(startNote, noteMetrics);
            var thisStatus = NoteAnimationHelper.GetOnStageStatusOf(startNote, now, tp1);

            if (thisStatus < OnStageStatus.Passed) {
                return GetHoldRibbonParameters(startNote, endNote, now, noteMetrics, animationMetrics);
            }

            var tp2 = NoteAnimationHelper.CalculateNoteTimePoints(endNote, noteMetrics);

            var t1 = GetTransformedTime(startNote, now, tp1);
            var t2 = GetTransformedTime(endNote, now, tp2);
            var tperc = startNote.IsHold() ? 0.5 : 0.4;
            var tm = MathHelper.Lerp(t1, t2, tperc);

            var trackCount = animationMetrics.TrackCount;
            var leftMarginRatio = animationMetrics.NoteEndXRatios[0];
            var rightMarginRatio = animationMetrics.NoteEndXRatios[trackCount - 1];
            var startXRatio = leftMarginRatio + (rightMarginRatio - leftMarginRatio) * (startNote.EndX / (trackCount - 1));
            var endXRatio = leftMarginRatio + (rightMarginRatio - leftMarginRatio) * (endNote.EndX / (trackCount - 1));

            var perc = (float)((now - startNote.HitTime) / (endNote.HitTime - startNote.HitTime));
            var x1Ratio = MathHelper.Lerp(startXRatio, endXRatio, perc);

            var x1 = GetNoteX(endNote, now, noteMetrics, animationMetrics);
            var y1 = GetNoteOnStageY(t2, animationMetrics);
            var x2 = animationMetrics.ClientSize.Width * x1Ratio;
            var y2 = animationMetrics.Bottom;

            // CGSS-like
            //var xm = GetNoteOnStageX(endNote.StartX, endNote.EndX, tm, animationMetrics);
            // Naive guess
            //var xm = (x1 + x2) / 2;
            var xm = MathHelper.Lerp(x1, x2, 0.5f);
            if (startNote.IsSlide()) {
                if (endNote.EndX < startNote.EndX) {
                    xm -= animationMetrics.ClientSize.Width * 0.02f * (float)((tp2.Leave - now) / (tp2.Leave - tp1.Enter));
                } else if (endNote.EndX > startNote.EndX) {
                    xm += animationMetrics.ClientSize.Width * 0.02f * (float)((tp2.Leave - now) / (tp2.Leave - tp1.Enter));
                }
            }
            var ym = GetNoteOnStageY(tm, animationMetrics);
            var (cx1, cx2) = GetBezierFromQuadratic(x1, xm, x2);
            var (cy1, cy2) = GetBezierFromQuadratic(y1, ym, y2);
            return new RibbonParameters(x1, y1, cx1, cy1, cx2, cy2, x2, y2);
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

        private static double GetTransformedTime(RuntimeNote note, double now, NoteTimePoints timePoints, bool clampIncoming = true, bool clampPassed = true) {
            var timeRemaining = note.HitTime - now;

            double timeRemainingInWindow;
            if (clampIncoming && timeRemaining > timePoints.Duration) {
                timeRemainingInWindow = 1f;
            } else if (clampPassed && timeRemaining < 0f) {
                timeRemainingInWindow = 0f;
            } else {
                timeRemainingInWindow = timeRemaining / timePoints.Duration;
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
            var transformedTime = GetTransformedTime(note, now, timePoints);
            return GetNoteOnStageY(transformedTime, animationMetrics);
        }

        private static float GetNoteOnStageX(float start, float end, double transformedTime, NoteAnimationMetrics animationMetrics) {
            var transformedX = GetNoteTransformedX(transformedTime);

            var trackCount = animationMetrics.TrackCount;
            var startLeftMarginRatio = animationMetrics.NoteStartXRatios[0];
            var startRightMarginRatio = animationMetrics.NoteStartXRatios[trackCount - 1];
            var endLeftMarginRatio = animationMetrics.NoteEndXRatios[0];
            var endRightMarginRatio = animationMetrics.NoteEndXRatios[trackCount - 1];

            if (start < 0) {
                start *= 0.5f;
            }

            var startXRatio = startLeftMarginRatio + (startRightMarginRatio - startLeftMarginRatio) * (start / (trackCount - 1));
            var endXRatio = endLeftMarginRatio + (endRightMarginRatio - endLeftMarginRatio) * (end / (trackCount - 1));

            var xRatio = endXRatio - (endXRatio - startXRatio) * transformedX;
            return animationMetrics.ClientSize.Width * xRatio;
        }

        private static (float ControlX1, float ControlX2) GetBezierFromQuadratic(float x1, float xmid, float x2) {
            var xcontrol = xmid * 2f - (x1 + x2) * 0.5f;
            var cx1 = (x1 + xcontrol * 2f) / 3f;
            var cx2 = (x2 + xcontrol * 2f) / 3f;
            return (cx1, cx2);
        }

    }
}
