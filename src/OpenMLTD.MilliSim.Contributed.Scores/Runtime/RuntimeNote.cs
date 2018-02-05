using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Contributed.Scores.Runtime {
    /// <summary>
    /// Represents a note read and played by the player.
    /// </summary>
    public class RuntimeNote {

        /// <summary>
        /// Note ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Absolute hit time, in seconds.
        /// </summary>
        public float HitTime { get; set; }

        /// <summary>
        /// The duration, starting from this note enters the stage, to this note exits the stage (reaches <see cref="HitTime"/>), in seconds.
        /// The actual value will be modified with <see cref="RelativeSpeed"/>.
        /// </summary>
        public float LeadTime { get; set; }

        /// <summary>
        /// The starting X position value. This value relies on track index to calculate the absolute X position on stage.
        /// An integer value N (zero-based) means it is at track N's position. A floating point value R is linearly interpolated
        /// between its two nearest integers.
        /// </summary>
        public float StartX { get; set; }

        /// <summary>
        /// The ending X position value. This value relies on track index to calculate the absolute X position on stage.
        /// An integer value N (zero-based) means it is at track N's position. A floating point value R is linearly interpolated
        /// between its two nearest integers.
        /// </summary>
        public float EndX { get; set; }

        /// <summary>
        /// The relative speed of this note to the global speed. When <see cref="RelativeSpeed"/> is 1.0, the speed of this note
        /// is the same as indicated by global speed. A higher value means falling faster.
        /// The empirical formula is [final speed] = [global speed] ^ 3 * [relative speed] ^ 2.
        /// </summary>
        public float RelativeSpeed { get; set; }

        /// <summary>
        /// The type of this note.
        /// </summary>
        public NoteType Type { get; set; }

        /// <summary>
        /// Flick direction of this note.
        /// </summary>
        public FlickDirection FlickDirection { get; set; }

        /// <summary>
        /// Size of this note.
        /// </summary>
        public NoteSize Size { get; set; }

        /// <summary>
        /// Flick/slide group ID. Reserved for CGSS scores.
        /// </summary>
        public int GroupID { get; set; }

        /// <summary>
        /// Previous synchronized Note.
        /// </summary>
        [CanBeNull]
        public RuntimeNote PrevSync { get; set; }

        /// <summary>
        /// Next synchronized Note.
        /// </summary>
        [CanBeNull]
        public RuntimeNote NextSync { get; set; }

        /// <summary>
        /// Previous Hold Note.
        /// </summary>
        [CanBeNull]
        public RuntimeNote PrevHold { get; set; }

        /// <summary>
        /// Next Hold Note.
        /// </summary>
        [CanBeNull]
        public RuntimeNote NextHold { get; set; }

        /// <summary>
        /// Previous Flick Note.
        /// </summary>
        [CanBeNull]
        public RuntimeNote PrevFlick { get; set; }

        /// <summary>
        /// Next Flick Note.
        /// </summary>
        [CanBeNull]
        public RuntimeNote NextFlick { get; set; }

        /// <summary>
        /// Previous Slide Note.
        /// </summary>
        [CanBeNull]
        public RuntimeNote PrevSlide { get; set; }

        /// <summary>
        /// Next Slide Note.
        /// </summary>
        [CanBeNull]
        public RuntimeNote NextSlide { get; set; }

        /// <summary>
        /// Extra information stored on this note. The player may recognize and use the values stored in this <see cref="IDynamic"/> to achieve advanced effects.
        /// This property is always set to <see langword="null"/> by <see cref="ScoreCompileHelper"/>.
        /// </summary>
        [CanBeNull]
        public IDynamic ExtraInfo { get; set; }

    }
}
