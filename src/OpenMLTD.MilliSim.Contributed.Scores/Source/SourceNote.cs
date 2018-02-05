using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Contributed.Scores.Source {
    public class SourceNote : NoteBase {

        /// <summary>
        /// Index of the track which this note is on.
        /// </summary>
        public int TrackIndex { get; set; }

        /// <summary>
        /// Base type of this note.
        /// </summary>
        public NoteType Type { get; set; }

        /// <summary>
        /// Start X position of this note.
        /// </summary>
        public float StartX { get; set; }

        /// <summary>
        /// End X position of this note.
        /// </summary>
        public float EndX { get; set; }

        /// <summary>
        /// Falling speed of this note.
        /// </summary>
        /// <remarks>
        /// There is also a speed table at the end of each beatmap. Not sure how that mapping goes.
        /// </remarks>
        public float Speed { get; set; }

        /// <summary>
        /// An array of <see cref="SourceNote"/>s following this note. For a Hold Start, Flick Start or Slide Start note, this array must contain
        /// at least 1 <see cref="SourceNote"/>.
        /// </summary>
        /// <remarks>
        /// <para>For example, a standard Hold Start note in MLTD/CGSS has 1 following note.</para>
        /// <para>Each following note's <see cref="SourceNote.Ticks"/> property should be an absolute value, not a relative value to this note.</para>
        /// <para>Types of notes in this array may differ. A slide group in CGSS can end in a slide note (normal slide group) or a series of flick notes ("tail flicking"). Both situtations can be represented by <see cref="FollowingNotes"/>.</para>
        /// </remarks>
        [CanBeNull, ItemNotNull]
        public SourceNote[] FollowingNotes { get; set; }

        /// <summary>
        /// Flick direction of this this note.
        /// </summary>
        public FlickDirection FlickDirection { get; set; }

        /// <summary>
        /// Size of this note.
        /// </summary>
        public NoteSize Size { get; set; }

        /// <summary>
        /// The standard time from the note enters the stage to it leaves the stage.
        /// The effective value is adjusted by a global speed scale, and a local speed scale (<see cref="Speed"/>).
        /// </summary>
        public float LeadTime { get; set; }

        /// <summary>
        /// Extra: group ID. Meant to be compatible with CGSS. Ignored by MLTD.
        /// </summary>
        public int GroupID { get; set; }

    }
}
