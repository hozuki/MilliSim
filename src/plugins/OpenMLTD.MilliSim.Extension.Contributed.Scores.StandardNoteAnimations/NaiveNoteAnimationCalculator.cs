using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Contributed.Scores.Animation;
using OpenMLTD.MilliSim.Contributed.Scores.Extending;
using OpenMLTD.MilliSim.Contributed.Scores.Extensions;
using OpenMLTD.MilliSim.Contributed.Scores.Runtime;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardNoteAnimations {
    /// <inheritdoc cref="NoteAnimationCalculator"/>
    /// <summary>
    /// This class calculates pseudo-static note traces. In other words, the notes will move downwards as if they are
    /// on a static beatmap image.
    /// </summary>
    [MilliSimPlugin(typeof(INoteAnimationCalculator))]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class NaiveNoteTraceCalculator : NoteAnimationCalculator {

        public override string PluginID => "plugin.scores.note_animation_calculator.naive";

        public override string PluginName => "Naive Note Animation Calculator";

        public override string PluginDescription => "Score viewing style note animation calculator.";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override Vector2 GetNoteRadius(RuntimeNote note, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            return noteMetrics.EndRadius;
        }

        public override float GetNoteX(RuntimeNote note, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
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
                    var nextPerc = (now - note.HitTime) / (note.NextSlide.HitTime - note.HitTime);
                    xRatio = MathHelperEx.Lerp(endXRatio, destXRatio, nextPerc);
                    break;
                default:
                    xRatio = endXRatio;
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
                    y = MathHelperEx.Lerp(animationMetrics.Top, animationMetrics.Bottom, (now - timePoints.Enter) / timePoints.Duration);
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
            var x1 = GetNoteX(startNote, now, noteMetrics, animationMetrics);
            var y1 = GetNoteY(startNote, now, noteMetrics, animationMetrics);
            var x2 = GetNoteX(endNote, now, noteMetrics, animationMetrics);
            var y2 = GetNoteY(endNote, now, noteMetrics, animationMetrics);
            return new RibbonParameters(x1, y1, x2, y2);
        }

        public override RibbonParameters GetFlickRibbonParameters(RuntimeNote startNote, RuntimeNote endNote, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var x1 = GetNoteX(startNote, now, noteMetrics, animationMetrics);
            var y1 = GetNoteY(startNote, now, noteMetrics, animationMetrics);
            var x2 = GetNoteX(endNote, now, noteMetrics, animationMetrics);
            var y2 = GetNoteY(endNote, now, noteMetrics, animationMetrics);
            return new RibbonParameters(x1, y1, x2, y2);
        }

        public override RibbonParameters GetSlideRibbonParameters(RuntimeNote startNote, RuntimeNote endNote, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var x1 = GetNoteX(startNote, now, noteMetrics, animationMetrics);
            var y1 = GetNoteY(startNote, now, noteMetrics, animationMetrics);
            var x2 = GetNoteX(endNote, now, noteMetrics, animationMetrics);
            var y2 = GetNoteY(endNote, now, noteMetrics, animationMetrics);
            return new RibbonParameters(x1, y1, x2, y2);
        }

        private static float GetIncomingNoteXRatio([NotNull] RuntimeNote prevNote, RuntimeNote thisNote, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics) {
            var trackCount = animationMetrics.TrackCount;
            var trackXRatioStart = animationMetrics.NoteEndXRatios[0];
            var trackXRatioEnd = animationMetrics.NoteEndXRatios[trackCount - 1];

            var thisXRatio = trackXRatioStart + (trackXRatioEnd - trackXRatioStart) * (prevNote.EndX / (trackCount - 1));
            var nextXRatio = trackXRatioStart + (trackXRatioEnd - trackXRatioStart) * (thisNote.EndX / (trackCount - 1));

            var thisTimePoints = NoteAnimationHelper.CalculateNoteTimePoints(prevNote, animationMetrics);
            var nextTimePoints = NoteAnimationHelper.CalculateNoteTimePoints(thisNote, animationMetrics);

            var perc = (now - thisTimePoints.Enter) / (nextTimePoints.Enter - thisTimePoints.Enter);
            return MathHelperEx.Lerp(thisXRatio, nextXRatio, perc);
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
