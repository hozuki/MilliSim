using System;
using System.Drawing;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Contributed.Scores.Animation;
using OpenMLTD.MilliSim.Contributed.Scores.Extensions;
using OpenMLTD.MilliSim.Contributed.Scores.Runtime;
using OpenMLTD.MilliSim.Plugin;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardNoteAnimations {
    /// <inheritdoc cref="NoteAnimationCalculator"/>
    /// <summary>
    /// This class calculates note traces in CGSS style.
    /// </summary>
    [MilliSimPlugin(typeof(INoteAnimationCalculator))]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class CgssNoteAnimationCalculator : NoteAnimationCalculator {

        public override string PluginID => "plugin.scores.note_animation_calculator.cgss";

        public override string PluginName => "CGSS Note Animation Calculator";

        public override string PluginDescription => "CGSS style note animation calculator.";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override SizeF GetNoteRadius(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var onStageStatus = NoteAnimationHelper.GetOnStageStatusOf(note, now, animationMetrics);

            switch (onStageStatus) {
                case OnStageStatus.Incoming:
                    return SizeF.Empty;
                case OnStageStatus.Passed:
                    return noteMetrics.EndRadius;
            }

            var timeRemaining = note.HitTime - now;
            var timePoints = NoteAnimationHelper.CalculateNoteTimePoints(note, animationMetrics);
            var timeTransformed = NoteTimeTransform((float)timeRemaining / timePoints.Duration);
            var endRadius = noteMetrics.EndRadius;

            if (timeTransformed < 0.75f) {
                if (timeTransformed < 0f) {
                    return endRadius;
                } else {
                    var r = 1 - (float)timeTransformed * 0.933333333f;
                    return new SizeF(endRadius.Width * r, endRadius.Height * r);
                }
            } else {
                if (timeTransformed < 1f) {
                    var r = (1 - (float)timeTransformed) * 1.2f;
                    return new SizeF(endRadius.Width * r, endRadius.Height * r);
                } else {
                    return SizeF.Empty;
                }
            }
        }

        public override float GetNoteX(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            if (note.IsSlide() && note.HasNextSlide()) {
                var thisStatus = NoteAnimationHelper.GetOnStageStatusOf(note, now, animationMetrics);
                if (thisStatus >= OnStageStatus.Passed) {
                    var nextSlide = note.NextSlide;
                    var nextStatus = NoteAnimationHelper.GetOnStageStatusOf(nextSlide, now, animationMetrics);
                    if (nextStatus < OnStageStatus.Passed) {
                        var x1 = GetEndXByNotePosition(note.EndX, animationMetrics);
                        var x2 = GetEndXByNotePosition(nextSlide.EndX, animationMetrics);
                        return (float)((now - note.HitTime) / (nextSlide.HitTime - note.HitTime)) * (x2 - x1) + x1;
                    }
                }
            }

            var transformedTime = GetTransformedTime(note, now, animationMetrics);
            return GetNoteXByTransformedTime(note, transformedTime, animationMetrics);
        }

        public override float GetNoteY(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            if (now >= note.HitTime) {
                return animationMetrics.Bottom;
            }

            var transformedTime = GetTransformedTime(note, now, animationMetrics);
            return GetNoteYByTransformedTime(transformedTime, animationMetrics);
        }

        public override SizeF GetSpecialNoteRadius(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            return GetNoteRadius(note, now, noteMetrics, animationMetrics);
        }

        public override float GetSpecialNoteX(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var leftRatio = animationMetrics.NoteEndXRatios[0];
            var rightRatio = animationMetrics.NoteEndXRatios[animationMetrics.TrackCount - 1];
            var xRatio = (leftRatio + rightRatio) / 2;
            return animationMetrics.Width * xRatio;
        }

        public override float GetSpecialNoteY(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            return GetNoteY(note, now, noteMetrics, animationMetrics);
        }

        public override RibbonParameters GetHoldRibbonParameters(RuntimeNote startNote, RuntimeNote endNote, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var t1 = (float)GetTransformedTime(startNote, now, animationMetrics);
            var t2 = (float)GetTransformedTime(endNote, now, animationMetrics);
            var tmid = (t1 + t2) * 0.5f;

            var x1 = GetNoteXByTransformedTime(startNote, t1, animationMetrics);
            var x2 = GetNoteXByTransformedTime(endNote, t2, animationMetrics);
            var xmid = GetNoteXByTransformedTime(endNote, tmid, animationMetrics);

            var y1 = GetNoteYByTransformedTime(t1, animationMetrics);
            var y2 = GetNoteYByTransformedTime(t2, animationMetrics);
            var ymid = GetNoteYByTransformedTime(tmid, animationMetrics);

            var (controlX1, controlX2) = GetBezierFromQuadratic(x1, xmid, x2);
            var (controlY1, controlY2) = GetBezierFromQuadratic(y1, ymid, y2);

            return new RibbonParameters(x1, y1, controlX1, controlY1, controlX2, controlY2, x2, y2);
        }

        public override RibbonParameters GetFlickRibbonParameters(RuntimeNote startNote, RuntimeNote endNote, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var x1 = GetNoteX(startNote, now, noteMetrics, animationMetrics);
            var y1 = GetNoteY(startNote, now, noteMetrics, animationMetrics);
            var x2 = GetNoteX(endNote, now, noteMetrics, animationMetrics);
            var y2 = GetNoteY(endNote, now, noteMetrics, animationMetrics);
            return new RibbonParameters(x1, y1, x2, y2);
        }

        public override RibbonParameters GetSlideRibbonParameters(RuntimeNote startNote, RuntimeNote endNote, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var startStatus = NoteAnimationHelper.GetOnStageStatusOf(startNote, now, animationMetrics);

            if (startNote.IsSlideEnd() || startStatus < OnStageStatus.Passed) {
                return GetHoldRibbonParameters(startNote, endNote, now, noteMetrics, animationMetrics);
            }

            var startX1 = GetEndXByNotePosition(startNote.EndX, animationMetrics);
            var startX2 = GetEndXByNotePosition(endNote.EndX, animationMetrics);

            var y1 = animationMetrics.Bottom;
            var x1 = (float)((now - startNote.HitTime) / (endNote.HitTime - startNote.HitTime)) * (startX2 - startX1) + startX1;

            var t1 = GetTransformedTime(startNote, now, animationMetrics);
            var t2 = GetTransformedTime(endNote, now, animationMetrics);
            var tmid = (t1 + t2) * 0.5f;

            var x2 = GetNoteXByTransformedTime(endNote, t2, animationMetrics);
            var xmid = GetNoteXByTransformedTime(endNote, tmid, animationMetrics);

            var y2 = GetNoteYByTransformedTime(t2, animationMetrics);
            var ymid = GetNoteYByTransformedTime(tmid, animationMetrics);

            var (controlX1, controlX2) = GetBezierFromQuadratic(x1, xmid, x2);
            var (controlY1, controlY2) = GetBezierFromQuadratic(y1, ymid, y2);

            return new RibbonParameters(x1, y1, controlX1, controlY1, controlX2, controlY2, x2, y2);
        }

        private static double NoteTimeTransform(double timeRemainingInWindow) {
            return timeRemainingInWindow / (2 - timeRemainingInWindow);
        }

        private static float NoteXTransform(double timeTransformed) {
            return (float)timeTransformed;
        }

        private static float NoteYTransform(double timeTransformed) {
            return (float)(timeTransformed + 2.05128205 * timeTransformed * (1 - timeTransformed));
        }

        private static float GetNoteXByTransformedTime(RuntimeNote note, double transformedTime, NoteAnimationMetrics animationMetrics) {
            var trackCount = animationMetrics.TrackCount;
            var startLeftMarginRatio = animationMetrics.NoteStartXRatios[0];
            var startRightMarginRatio = animationMetrics.NoteStartXRatios[trackCount - 1];
            var endLeftMarginRatio = animationMetrics.NoteEndXRatios[0];
            var endRightMarginRatio = animationMetrics.NoteEndXRatios[trackCount - 1];

            var startXRatio = startLeftMarginRatio + (startRightMarginRatio - startLeftMarginRatio) * (note.StartX / (trackCount - 1));
            var endXRatio = endLeftMarginRatio + (endRightMarginRatio - endLeftMarginRatio) * (note.EndX / (trackCount - 1));

            var startX = animationMetrics.Width * startXRatio;
            var endX = animationMetrics.Width * endXRatio;

            return endX - (endX - startX) * NoteXTransform(transformedTime);
        }

        private static float GetNoteYByTransformedTime(double transformedTime, NoteAnimationMetrics animationMetrics) {
            return animationMetrics.Bottom - (animationMetrics.Bottom - animationMetrics.Top) * NoteYTransform(transformedTime);
        }

        private static float GetEndXByNotePosition(float noteX, NoteAnimationMetrics animationMetrics) {
            var trackCount = animationMetrics.TrackCount;
            var endLeftMarginRatio = animationMetrics.NoteEndXRatios[0];
            var endRightMarginRatio = animationMetrics.NoteEndXRatios[trackCount - 1];
            var endXRatio = endLeftMarginRatio + (endRightMarginRatio - endLeftMarginRatio) * (noteX / (trackCount - 1));
            return animationMetrics.Width * endXRatio;
        }

        private static double GetTransformedTime(RuntimeNote note, double now, NoteAnimationMetrics animationMetrics) {
            var timePoints = NoteAnimationHelper.CalculateNoteTimePoints(note, animationMetrics);
            var timeRemaining = note.HitTime - now;
            var timeRemainingInWindow = (float)timeRemaining / timePoints.Duration;

            if (timeRemaining > timePoints.Duration) {
                timeRemainingInWindow = 1;
            }

            if (timeRemaining < 0) {
                timeRemainingInWindow = 0;
            }

            return NoteTimeTransform(timeRemainingInWindow);
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
