using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Contributed.Scores.Animation;
using OpenMLTD.MilliSim.Contributed.Scores.Extensions;
using OpenMLTD.MilliSim.Contributed.Scores.Runtime;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Plugin;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardNoteAnimations {
    /// <inheritdoc cref="NoteAnimationCalculator"/>
    /// <summary>
    /// This class calculates note traces in MLTD style.
    /// </summary>
    [MilliSimPlugin(typeof(INoteAnimationCalculator))]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class MltdNoteTraceCalculator : NoteAnimationCalculator {

        public override string PluginID => "plugin.scores.note_animation_calculator.mltd";

        public override string PluginName => "MLTD Note Animation Calculator";

        public override string PluginDescription => "MLTD style note animation calculator.";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override Vector2 GetNoteRadius(RuntimeNote note, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var timePoints = NoteAnimationHelper.CalculateNoteTimePoints(note, animationMetrics);
            var onStageStatus = NoteAnimationHelper.GetOnStageStatusOf(note, now, timePoints);
            switch (onStageStatus) {
                case OnStageStatus.Incoming:
                    return noteMetrics.StartRadius;
                case OnStageStatus.Visible:
                    var passed = now - timePoints.Enter;
                    var perc = passed / timePoints.Duration;
                    var w = MathHelperEx.Lerp(noteMetrics.StartRadius.X, noteMetrics.EndRadius.X, perc);
                    var h = MathHelperEx.Lerp(noteMetrics.StartRadius.Y, noteMetrics.EndRadius.Y, perc);
                    return new Vector2(w, h);
                case OnStageStatus.Passed:
                    return noteMetrics.EndRadius;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override float GetNoteX(RuntimeNote note, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var trackCount = animationMetrics.TrackCount;
            var endLeftMarginRatio = animationMetrics.NoteEndXRatios[0];
            var endRightMarginRatio = animationMetrics.NoteEndXRatios[trackCount - 1];

            var endXRatio = endLeftMarginRatio + (endRightMarginRatio - endLeftMarginRatio) * (note.EndX / (trackCount - 1));

            var onStage = NoteAnimationHelper.GetOnStageStatusOf(note, now, animationMetrics);
            float xRatio;
            switch (onStage) {
                case OnStageStatus.Incoming:
                    if (note.HasPrevHold()) {
                        xRatio = GetIncomingNoteXRatio(note.PrevHold, note, now, animationMetrics);
                    } else if (note.HasPrevSlide()) {
                        xRatio = GetIncomingNoteXRatio(note.PrevSlide, note, now, animationMetrics);
                    } else {
                        xRatio = endXRatio;
                    }
                    break;
                case OnStageStatus.Passed:
                    if (note.HasNextSlide()) {
                        var destXRatio = endLeftMarginRatio + (endRightMarginRatio - endLeftMarginRatio) * (note.NextSlide.EndX / (trackCount - 1));
                        var nextPerc = (now - note.HitTime) / (note.NextSlide.HitTime - note.HitTime);
                        xRatio = MathHelperEx.Lerp(endXRatio, destXRatio, nextPerc);
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
                        if (note.StartX < 0) {
                            whichStartToTake = note.StartX * 0.5f;
                        } else if (note.StartX > trackCount - 1) {
                            whichStartToTake = (trackCount - 1) + (note.StartX - (trackCount - 1)) * 0.5f;
                        } else {
                            whichStartToTake = note.StartX;
                        }
                    }

                    var startXRatio = startLeftMarginRatio + (startRightMarginRatio - startLeftMarginRatio) * (whichStartToTake / (trackCount - 1));

                    var timePoints = NoteAnimationHelper.CalculateNoteTimePoints(note, animationMetrics);
                    var perc = (now - timePoints.Enter) / timePoints.Duration;

                    xRatio = MathHelperEx.Lerp(startXRatio, endXRatio, perc);
                    break;
            }

            return animationMetrics.Width * xRatio;
        }

        public override float GetNoteY(RuntimeNote note, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var timePoints = NoteAnimationHelper.CalculateNoteTimePoints(note, animationMetrics);
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

        public override Vector2 GetSpecialNoteRadius(RuntimeNote note, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            return GetNoteRadius(note, now, noteMetrics, animationMetrics);
        }

        public override float GetSpecialNoteX(RuntimeNote note, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var leftRatio = animationMetrics.NoteEndXRatios[0];
            var rightRatio = animationMetrics.NoteEndXRatios[animationMetrics.TrackCount - 1];
            var xRatio = (leftRatio + rightRatio) / 2;
            return animationMetrics.Width * xRatio;
        }

        public override float GetSpecialNoteY(RuntimeNote note, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            return GetNoteY(note, now, noteMetrics, animationMetrics);
        }

        public override RibbonParameters GetHoldRibbonParameters(RuntimeNote startNote, RuntimeNote endNote, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var tp1 = NoteAnimationHelper.CalculateNoteTimePoints(startNote, animationMetrics);
            var tp2 = NoteAnimationHelper.CalculateNoteTimePoints(endNote, animationMetrics);

            var t1 = GetTransformedTime(startNote, now, tp1);
            var t2 = GetTransformedTime(endNote, now, tp2);
            var tperc = startNote.IsHold() ? 0.5 : 0.4;
            var tm = MathHelperEx.Lerp(t1, t2, tperc);

            var x1 = GetNoteX(startNote, now, noteMetrics, animationMetrics);
            var x2 = GetNoteX(endNote, now, noteMetrics, animationMetrics);

            var startStatus = NoteAnimationHelper.GetOnStageStatusOf(startNote, now, tp1);
            float y1;
            if (startStatus == OnStageStatus.Passed) {
                y1 = animationMetrics.Bottom;
            } else {
                y1 = GetNoteOnStageY(t1, animationMetrics);
            }

            var endStatus = NoteAnimationHelper.GetOnStageStatusOf(endNote, now, tp2);
            float y2;
            if (endStatus == OnStageStatus.Incoming) {
                y2 = animationMetrics.Top;
            } else {
                y2 = GetNoteOnStageY(t2, animationMetrics);
            }

            // CGSS-like
            //var xm = GetNoteOnStageX(endNote.StartX, endNote.EndX, tm, animationMetrics);
            // Naive guess
            //var xm = (x1 + x2) / 2;
            var xm = MathHelperEx.Lerp(x1, x2, 0.5f);
            if (startNote.IsSlide()) {
                if (endNote.EndX < startNote.EndX) {
                    xm -= animationMetrics.Width * 0.02f * ((tp2.Leave - now) / (tp2.Leave - tp1.Enter));
                } else if (endNote.EndX > startNote.EndX) {
                    xm += animationMetrics.Width * 0.02f * ((tp2.Leave - now) / (tp2.Leave - tp1.Enter));
                }
            }
            var ym = GetNoteOnStageY(tm, animationMetrics);
            var (cx1, cx2) = GetBezierFromQuadratic(x1, xm, x2);
            var (cy1, cy2) = GetBezierFromQuadratic(y1, ym, y2);
            return new RibbonParameters(x1, y1, cx1, cy1, cx2, cy2, x2, y2);
        }

        public override RibbonParameters GetFlickRibbonParameters(RuntimeNote startNote, RuntimeNote endNote, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            return new RibbonParameters {
                Visible = false
            };
        }

        public override RibbonParameters GetSlideRibbonParameters(RuntimeNote startNote, RuntimeNote endNote, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var tp1 = NoteAnimationHelper.CalculateNoteTimePoints(startNote, animationMetrics);
            var thisStatus = NoteAnimationHelper.GetOnStageStatusOf(startNote, now, tp1);

            if (thisStatus < OnStageStatus.Passed) {
                return GetHoldRibbonParameters(startNote, endNote, now, noteMetrics, animationMetrics);
            }

            var tp2 = NoteAnimationHelper.CalculateNoteTimePoints(endNote, animationMetrics);

            var t1 = GetTransformedTime(startNote, now, tp1);
            var t2 = GetTransformedTime(endNote, now, tp2);
            var tperc = startNote.IsHold() ? 0.5 : 0.4;
            var tm = MathHelperEx.Lerp(t1, t2, tperc);

            var trackCount = animationMetrics.TrackCount;
            var leftMarginRatio = animationMetrics.NoteEndXRatios[0];
            var rightMarginRatio = animationMetrics.NoteEndXRatios[trackCount - 1];
            var startXRatio = leftMarginRatio + (rightMarginRatio - leftMarginRatio) * (startNote.EndX / (trackCount - 1));
            var endXRatio = leftMarginRatio + (rightMarginRatio - leftMarginRatio) * (endNote.EndX / (trackCount - 1));

            var perc = (now - startNote.HitTime) / (endNote.HitTime - startNote.HitTime);
            var x1Ratio = MathHelperEx.Lerp(startXRatio, endXRatio, perc);

            var x1 = animationMetrics.Width * x1Ratio;
            var y1 = animationMetrics.Bottom;
            var x2 = GetNoteX(endNote, now, noteMetrics, animationMetrics);
            var y2 = GetNoteOnStageY(t2, animationMetrics);

            // CGSS-like
            //var xm = GetNoteOnStageX(endNote.StartX, endNote.EndX, tm, animationMetrics);
            // Naive guess
            //var xm = (x1 + x2) / 2;
            var xm = MathHelperEx.Lerp(x1, x2, 0.5f);
            if (startNote.IsSlide()) {
                if (endNote.EndX < startNote.EndX) {
                    xm -= animationMetrics.Width * 0.02f * ((tp2.Leave - now) / (tp2.Leave - tp1.Enter));
                } else if (endNote.EndX > startNote.EndX) {
                    xm += animationMetrics.Width * 0.02f * ((tp2.Leave - now) / (tp2.Leave - tp1.Enter));
                }
            }
            var ym = GetNoteOnStageY(tm, animationMetrics);
            var (cx1, cx2) = GetBezierFromQuadratic(x1, xm, x2);
            var (cy1, cy2) = GetBezierFromQuadratic(y1, ym, y2);
            return new RibbonParameters(x1, y1, cx1, cy1, cx2, cy2, x2, y2);
        }

        private static float GetIncomingNoteXRatio(RuntimeNote prevNote, RuntimeNote thisNote, float now, NoteAnimationMetrics animationMetrics) {
            var trackCount = animationMetrics.TrackCount;
            var startLeftMarginRatio = animationMetrics.NoteStartXRatios[0];
            var startRightMarginRatio = animationMetrics.NoteStartXRatios[trackCount - 1];

            float xRatio;
            if (thisNote.IsSlide()) {
                var thisXRatio = startLeftMarginRatio + (startRightMarginRatio - startLeftMarginRatio) * (prevNote.EndX / (trackCount - 1));
                var nextXRatio = startLeftMarginRatio + (startRightMarginRatio - startLeftMarginRatio) * (thisNote.EndX / (trackCount - 1));

                var thisTimePoints = NoteAnimationHelper.CalculateNoteTimePoints(prevNote, animationMetrics);
                var nextTimePoints = NoteAnimationHelper.CalculateNoteTimePoints(thisNote, animationMetrics);

                var perc = (now - thisTimePoints.Enter) / (nextTimePoints.Enter - thisTimePoints.Enter);
                xRatio = MathHelperEx.Lerp(thisXRatio, nextXRatio, perc);

            } else {
                float nextStartX;
                if (thisNote.StartX < 0) {
                    nextStartX = thisNote.StartX * 0.5f;
                } else if (thisNote.StartX > trackCount - 1) {
                    nextStartX = (trackCount - 1) + (thisNote.StartX - (trackCount - 1)) * 0.5f;
                } else {
                    nextStartX = thisNote.StartX;
                }
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

        private static (float ControlX1, float ControlX2) GetBezierFromQuadratic(float x1, float xmid, float x2) {
            var xcontrol = xmid * 2f - (x1 + x2) * 0.5f;
            var cx1 = (x1 + xcontrol * 2f) / 3f;
            var cx2 = (x2 + xcontrol * 2f) / 3f;
            return (cx1, cx2);
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
