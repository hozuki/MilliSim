using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core.Entities.Source {
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
        /// <see cref="PolyPoint"/> data of this note. For a Hold Start or Slide Start note, this array must contain
        /// at least 1 <see cref="PolyPoint"/>.
        /// </summary>
        [CanBeNull, ItemNotNull]
        public PolyPoint[] PolyPoints { get; set; }

        /// <summary>
        /// Flick direction of this this note. For a Hold Start or Slide Start note, this is the flick direction of corresponding Hold End or Slide End note.
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
        public double LeadTime { get; set; }

        /// <summary>
        /// Extra: group ID. Meant to be compatible with CGSS. Ignored by MLTD.
        /// </summary>
        public int GroupID { get; set; }

    }
}
