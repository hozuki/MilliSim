namespace OpenMLTD.MilliSim.Core.Entities.Source {
    /// <summary>
    /// The base class of all kinds of notes.
    /// </summary>
    public abstract class NoteBase {

        /// <summary>
        /// Index of the measure which this note is in. Zero-based.
        /// </summary>
        public int Measure { get; set; }

        /// <summary>
        /// Index of the beat which this note is on. Zero-based.
        /// </summary>
        public int Beat { get; set; }

        /// <summary>
        /// Accurate number of ticks. In the standard format, each beat is equally divided into <see cref="TicksPerBeat"/> ticks.
        /// </summary>
        public long Ticks { get; set; }

        /// <summary>
        /// Ticks per beat.
        /// </summary>
        public static readonly int TicksPerBeat = 96;

    }
}
