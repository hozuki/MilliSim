using System;
using System.Drawing;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Runtime.Extensions;
using OpenMLTD.MilliSim.Theater.Elements;

namespace OpenMLTD.MilliSim.Theater.Intenal {
    internal static class RuntimeNoteCalculator {

        internal static OnStageStatus GetOnStageStatusOf(RuntimeNote note, double currentSecond, double enter, double leave) {
            if (currentSecond < enter) {
                return OnStageStatus.Incoming;
            } else if (currentSecond > leave) {
                return OnStageStatus.Left;
            } else {
                return OnStageStatus.OnStage;
            }
        }

        internal static OnStageStatus GetOnStageStatusOf(RuntimeNote note, double currentSecond, float speedScale) {
            var (enter, leave, _) = CalculateNoteTimePoints(note, speedScale);
            return GetOnStageStatusOf(note, currentSecond, enter, leave);
        }

        internal static (double EnterAbsolute, double LeaveAbsolute, float LeadScaled) CalculateNoteTimePoints(RuntimeNote note, float speedScale) {
            // Empirical formula: s = pow(game_setting, 3) * pow(note_speed, 2)
            var absoluteSpeed = speedScale * speedScale * speedScale * note.RelativeSpeed * note.RelativeSpeed;
            var leadTimeScaled = (float)note.LeadTime / absoluteSpeed;
            return (note.HitTime - leadTimeScaled, note.HitTime, leadTimeScaled);
        }

        internal static float CalculateNoteX(RuntimeNote note, double currentSecond, float[] startXRatios, float[] endXRatios, Size clientSize, float speedScale, ScoreRenderMode renderMode) {
            var (enter, leave, lead) = CalculateNoteTimePoints(note, speedScale);
            return CalculateNoteX(note, currentSecond, startXRatios, endXRatios, clientSize, enter, leave, lead, renderMode);
        }

        internal static float CalculateNoteX(RuntimeNote note, double currentSecond, float[] startXRatios, float[] endXRatios, Size clientSize, double enter, double leave, double lead, ScoreRenderMode renderMode) {
            var trackCount = endXRatios.Length;
            var trackXRatioStart = endXRatios[0];
            var trackXRatioEnd = endXRatios[trackCount - 1];

            var endXRatio = trackXRatioStart + (trackXRatioEnd - trackXRatioStart) * (note.EndX / (trackCount - 1));

            float xRatio;
            switch (renderMode) {
                case ScoreRenderMode.Standard:
                    var perc = ((float)((currentSecond - enter) / lead)).Clamp(0, 1);
                    var startXRatio = trackXRatioStart + (trackXRatioEnd - trackXRatioStart) * (note.StartX / (trackCount - 1));
                    xRatio = MathHelper.Lerp(startXRatio, endXRatio, perc);
                    break;
                case ScoreRenderMode.Straight:
                    xRatio = endXRatio;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(renderMode), renderMode, null);
            }

            return clientSize.Width * xRatio;
        }

        internal static float CalculateRibbonX(RuntimeNote note, double currentSecond, float[] startXRatios, float[] endXRatios, Size clientSize, float speedScale, ScoreRenderMode renderMode) {
            var (enter, leave, lead) = CalculateNoteTimePoints(note, speedScale);
            return CalculateRibbonX(note, currentSecond, startXRatios, endXRatios, clientSize, enter, leave, lead, renderMode);
        }

        internal static float CalculateRibbonX(RuntimeNote note, double currentSecond, float[] startXRatios, float[] endXRatios, Size clientSize, double enter, double leave, double lead, ScoreRenderMode renderMode) {
            var trackCount = endXRatios.Length;
            var trackXRatioStart = endXRatios[0];
            var trackXRatioEnd = endXRatios[trackCount - 1];

            var endXRatio = trackXRatioStart + (trackXRatioEnd - trackXRatioStart) * (note.EndX / (trackCount - 1));

            float xRatio;
            switch (renderMode) {
                case ScoreRenderMode.Standard:
                    var thisPerc = ((float)((currentSecond - enter) / lead)).Clamp(0, 1);
                    var startXRatio = trackXRatioStart + (trackXRatioEnd - trackXRatioStart) * (note.StartX / (trackCount - 1));
                    xRatio = MathHelper.Lerp(startXRatio, endXRatio, thisPerc);
                    break;
                case ScoreRenderMode.Straight:
                    xRatio = endXRatio;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(renderMode), renderMode, null);
            }

            var thisX = clientSize.Width * xRatio;

            var onStage = GetOnStageStatusOf(note, currentSecond, enter, leave);
            if (onStage == OnStageStatus.Left && note.HasNextSlide()) {
                var destXRatio = trackXRatioStart + (trackXRatioEnd - trackXRatioStart) * (note.NextSlide.EndX / (trackCount - 1));
                var destX = clientSize.Width * destXRatio;
                var nextPerc = (float)(currentSecond - note.HitTime) / (float)(note.NextSlide.HitTime - note.HitTime);
                return MathHelper.Lerp(thisX, destX, nextPerc);
            } else {
                return thisX;
            }
        }

        internal static (float Y, bool Visible) CalculateNoteY(RuntimeNote note, double currentSecond, float topY, float bottomY, float speedScale, ScoreRenderMode renderMode, bool fastFail) {
            var (enter, leave, lead) = CalculateNoteTimePoints(note, speedScale);
            return CalculateNoteY(note, currentSecond, topY, bottomY, enter, leave, lead, renderMode, fastFail);
        }

        internal static (float Y, bool Visible) CalculateNoteY(RuntimeNote note, double currentSecond, float topY, float bottomY, double enter, double leave, float lead, ScoreRenderMode renderMode, bool fastFail) {
            var onStageStatus = GetOnStageStatusOf(note, currentSecond, enter, leave);

            if (onStageStatus != OnStageStatus.OnStage) {
                if (fastFail) {
                    return (0, false);
                }
            }

            float y;
            switch (onStageStatus) {
                case OnStageStatus.Incoming:
                    y = topY;
                    break;
                case OnStageStatus.OnStage:
                    y = MathHelper.Lerp(topY, bottomY, (float)(currentSecond - enter) / lead);
                    break;
                case OnStageStatus.Left:
                    y = bottomY;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return (y, true);
        }

        internal static (float X, float Y, bool Visible) CalculateNoteLocation(RuntimeNote note, double currentSecond, float[] startXRatios, float[] endXRatios, Size clientSize, float topY, float bottomY, float speedScale, ScoreRenderMode renderMode, bool fastFail) {
            var (enter, leave, lead) = CalculateNoteTimePoints(note, speedScale);
            var onStageStatus = GetOnStageStatusOf(note, currentSecond, enter, leave);

            if (onStageStatus != OnStageStatus.OnStage) {
                if (fastFail) {
                    return (0, 0, false);
                }
            }

            var x = CalculateNoteX(note, currentSecond, startXRatios, endXRatios, clientSize, enter, leave, lead, renderMode);

            var (y, v) = CalculateNoteY(note, currentSecond, topY, bottomY, enter, leave, lead, renderMode, fastFail);

            return (x, y, v);
        }

        internal static (float X, float Y, bool Visible) CalculateRibbonLocation(RuntimeNote note, double currentSecond, float[] startXRatios, float[] endXRatios, Size clientSize, float topY, float bottomY, float speedScale, ScoreRenderMode renderMode, bool fastFail) {
            var (enter, leave, lead) = CalculateNoteTimePoints(note, speedScale);
            var onStageStatus = GetOnStageStatusOf(note, currentSecond, enter, leave);

            if (onStageStatus != OnStageStatus.OnStage) {
                if (fastFail) {
                    return (0, 0, false);
                }
            }

            var x = CalculateRibbonX(note, currentSecond, startXRatios, endXRatios, clientSize, enter, leave, lead, renderMode);

            var (y, v) = CalculateNoteY(note, currentSecond, topY, bottomY, enter, leave, lead, renderMode, fastFail);

            return (x, y, v);
        }

    }
}
