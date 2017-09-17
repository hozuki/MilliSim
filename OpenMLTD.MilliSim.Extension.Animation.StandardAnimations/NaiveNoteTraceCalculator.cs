using System;
using System.Composition;
using System.Drawing;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Runtime.Extensions;
using OpenMLTD.MilliSim.Theater.Animation;
using OpenMLTD.MilliSim.Theater.Animation.Extending;

namespace OpenMLTD.MilliSim.Extension.Animation.StandardAnimations {
    /// <inheritdoc cref="NoteTraceCalculator"/>
    /// <summary>
    /// This class calculates pseudo-static note traces. In other words, the notes will move downwards as if they are
    /// on a static beatmap image.
    /// </summary>
    [Export(typeof(INoteTraceCalculator))]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class NaiveNoteTraceCalculator : NoteTraceCalculator {

        public override string PluginID => "plugin.animation.naive";

        public override string PluginName => "Naive Note Trace Calculator";

        public override string PluginDescription => "Score viewing style note animation calculator.";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override SizeF GetNoteRadius(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            return noteMetrics.EndRadius;
        }

        public override float GetNoteX(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var trackCount = animationMetrics.TrackCount;
            var trackXRatioStart = animationMetrics.NoteEndXRatios[0];
            var trackXRatioEnd = animationMetrics.NoteEndXRatios[trackCount - 1];

            var endXRatio = trackXRatioStart + (trackXRatioEnd - trackXRatioStart) * (note.EndX / (trackCount - 1));

            var onStage = NoteAnimationHelper.GetOnStageStatusOf(note, now, animationMetrics);
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
                case OnStageStatus.Passed when note.HasNextSlide():
                    var destXRatio = trackXRatioStart + (trackXRatioEnd - trackXRatioStart) * (note.NextSlide.EndX / (trackCount - 1));
                    var nextPerc = (float)(now - note.HitTime) / (float)(note.NextSlide.HitTime - note.HitTime);
                    xRatio = MathHelper.Lerp(endXRatio, destXRatio, nextPerc);
                    break;
                default:
                    xRatio = endXRatio;
                    break;
            }

            return animationMetrics.Width * xRatio;
        }

        public override float GetNoteY(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var timePoints = NoteAnimationHelper.CalculateNoteTimePoints(note, animationMetrics);
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
            var x1 = GetNoteX(startNote, now, noteMetrics, animationMetrics);
            var y1 = GetNoteY(startNote, now, noteMetrics, animationMetrics);
            var x2 = GetNoteX(endNote, now, noteMetrics, animationMetrics);
            var y2 = GetNoteY(endNote, now, noteMetrics, animationMetrics);
            return new RibbonParameters(x1, y1, x2, y2);
        }

        public override RibbonParameters GetSlideRibbonParameters(RuntimeNote startNote, RuntimeNote endNote, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var x1 = GetNoteX(startNote, now, noteMetrics, animationMetrics);
            var y1 = GetNoteY(startNote, now, noteMetrics, animationMetrics);
            var x2 = GetNoteX(endNote, now, noteMetrics, animationMetrics);
            var y2 = GetNoteY(endNote, now, noteMetrics, animationMetrics);
            return new RibbonParameters(x1, y1, x2, y2);
        }

        private static float GetIncomingNoteXRatio([NotNull] RuntimeNote prevNote, RuntimeNote thisNote, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var trackCount = animationMetrics.TrackCount;
            var trackXRatioStart = animationMetrics.NoteEndXRatios[0];
            var trackXRatioEnd = animationMetrics.NoteEndXRatios[trackCount - 1];

            var thisXRatio = trackXRatioStart + (trackXRatioEnd - trackXRatioStart) * (prevNote.EndX / (trackCount - 1));
            var nextXRatio = trackXRatioStart + (trackXRatioEnd - trackXRatioStart) * (thisNote.EndX / (trackCount - 1));

            var thisTimePoints = NoteAnimationHelper.CalculateNoteTimePoints(prevNote, animationMetrics);
            var nextTimePoints = NoteAnimationHelper.CalculateNoteTimePoints(thisNote, animationMetrics);

            var perc = (float)(now - thisTimePoints.Enter) / (float)(nextTimePoints.Enter - thisTimePoints.Enter);
            return MathHelper.Lerp(thisXRatio, nextXRatio, perc);
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
