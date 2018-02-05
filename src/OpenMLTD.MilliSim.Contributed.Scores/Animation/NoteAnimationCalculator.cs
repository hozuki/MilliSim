using System;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Contributed.Scores.Extending;
using OpenMLTD.MilliSim.Contributed.Scores.Runtime;

namespace OpenMLTD.MilliSim.Contributed.Scores.Animation {
    /// <inheritdoc cref="INoteAnimationCalculator"/>
    /// <summary>
    /// A base class implementing <see cref="INoteAnimationCalculator"/>.
    /// </summary>
    public abstract class NoteAnimationCalculator : INoteAnimationCalculator {

        public abstract string PluginID { get; }

        public abstract string PluginName { get; }

        public abstract string PluginDescription { get; }

        public abstract string PluginAuthor { get; }

        public abstract Version PluginVersion { get; }

        public string PluginCategory => "Note Animation Calculator";

        public abstract Vector2 GetNoteRadius(RuntimeNote note, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics);

        public abstract float GetNoteX(RuntimeNote note, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics);

        public abstract float GetNoteY(RuntimeNote note, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics);

        public abstract Vector2 GetSpecialNoteRadius(RuntimeNote note, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics);

        public abstract float GetSpecialNoteX(RuntimeNote note, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics);

        public abstract float GetSpecialNoteY(RuntimeNote note, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics);

        public abstract RibbonParameters GetHoldRibbonParameters(RuntimeNote startNote, RuntimeNote endNote, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics);

        public abstract RibbonParameters GetFlickRibbonParameters(RuntimeNote startNote, RuntimeNote endNote, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics);

        public abstract RibbonParameters GetSlideRibbonParameters(RuntimeNote startNote, RuntimeNote endNote, float now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics);

        public int ApiVersion => 2;

    }
}
