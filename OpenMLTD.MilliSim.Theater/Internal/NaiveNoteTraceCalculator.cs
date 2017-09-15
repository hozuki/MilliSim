using System;
using System.Drawing;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Runtime.Extensions;

namespace OpenMLTD.MilliSim.Theater.Internal {
    /// <inheritdoc cref="INoteTraceCalculator"/>
    /// <summary>
    /// This class calculates pseudo-static note traces. In other words, the notes will move downwards as if they are
    /// on a static beatmap image.
    /// </summary>
    internal sealed class NaiveNoteTraceCalculator : INoteTraceCalculator {

        public SizeF GetNoteRadius(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            return noteMetrics.EndRadius;
        }

        public float GetNoteX(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var trackCount = animationMetrics.TrackCount;
            var trackXRatioStart = animationMetrics.NoteEndXRatios[0];
            var trackXRatioEnd = animationMetrics.NoteEndXRatios[trackCount - 1];

            var endXRatio = trackXRatioStart + (trackXRatioEnd - trackXRatioStart) * (note.EndX / (trackCount - 1));

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
                case OnStageStatus.Passed when note.HasNextSlide():
                    var destXRatio = trackXRatioStart + (trackXRatioEnd - trackXRatioStart) * (note.NextSlide.EndX / (trackCount - 1));
                    var nextPerc = (float)(now - note.HitTime) / (float)(note.NextSlide.HitTime - note.HitTime);
                    xRatio = MathHelper.Lerp(endXRatio, destXRatio, nextPerc);
                    break;
                default:
                    xRatio = endXRatio;
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

        public (float X1, float Y1, float ControlX1, float ControlY1, float ControlX2, float ControlY2, float X2, float Y2) GetRibbonLocations(RuntimeNote thisNote, RuntimeNote nextNote, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            throw new NotImplementedException();
        }

        private static float GetIncomingNoteXRatio([NotNull] RuntimeNote prevNote, RuntimeNote thisNote, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var trackCount = animationMetrics.TrackCount;
            var trackXRatioStart = animationMetrics.NoteEndXRatios[0];
            var trackXRatioEnd = animationMetrics.NoteEndXRatios[trackCount - 1];

            var thisXRatio = trackXRatioStart + (trackXRatioEnd - trackXRatioStart) * (prevNote.EndX / (trackCount - 1));
            var nextXRatio = trackXRatioStart + (trackXRatioEnd - trackXRatioStart) * (thisNote.EndX / (trackCount - 1));

            var thisTimePoints = NoteAnimationHelper.CalculateNoteTimePoints(prevNote, noteMetrics);
            var nextTimePoints = NoteAnimationHelper.CalculateNoteTimePoints(thisNote, noteMetrics);

            var perc = (float)(now - thisTimePoints.Enter) / (float)(nextTimePoints.Enter - thisTimePoints.Enter);
            return MathHelper.Lerp(thisXRatio, nextXRatio, perc);
        }

    }
}
