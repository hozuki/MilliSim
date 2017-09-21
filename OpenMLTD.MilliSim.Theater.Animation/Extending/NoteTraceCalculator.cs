using System;
using System.Drawing;
using OpenMLTD.MilliSim.Core.Entities.Runtime;

namespace OpenMLTD.MilliSim.Theater.Animation.Extending {
    /// <inheritdoc cref="INoteTraceCalculator"/>
    /// <summary>
    /// A base class implementing <see cref="INoteTraceCalculator"/>.
    /// </summary>
    public abstract class NoteTraceCalculator : INoteTraceCalculator {

        public abstract string PluginID { get; }

        public abstract string PluginName { get; }

        public abstract string PluginDescription { get; }

        public abstract string PluginAuthor { get; }

        public abstract Version PluginVersion { get; }

        public string PluginCategory => "Animation";

        public abstract SizeF GetNoteRadius(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics);

        public abstract float GetNoteX(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics);

        public abstract float GetNoteY(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics);

        public abstract SizeF GetSpecialNoteRadius(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics);

        public abstract float GetSpecialNoteX(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics);

        public abstract float GetSpecialNoteY(RuntimeNote note, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics);

        public abstract RibbonParameters GetHoldRibbonParameters(RuntimeNote startNote, RuntimeNote endNote, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics);

        public abstract RibbonParameters GetFlickRibbonParameters(RuntimeNote startNote, RuntimeNote endNote, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics);

        public abstract RibbonParameters GetSlideRibbonParameters(RuntimeNote startNote, RuntimeNote endNote, double now, NoteMetrics noteMetrics, NoteAnimationMetrics animationMetrics);

        public int ApiVersion => 1;

    }
}
