using JetBrains.Annotations;
using OpenMLTD.MilliSim.Contributed.Scores.Runtime;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Plugin;

namespace OpenMLTD.MilliSim.Contributed.Scores.Source {
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
        /// Stores extra information in this note.
        /// </summary>
        /// <remarks>
        /// The information stored can be read by <see cref="IScoreCompiler"/>s to generate <see cref="RuntimeNote"/>s.
        /// This property is ignored by the default compile methods in <see cref="ScoreCompileHelper"/>.
        /// </remarks>
        [CanBeNull]
        public IDynamic ExtraInfo { get; set; }

        /// <summary>
        /// Ticks per beat.
        /// </summary>
        public static readonly int TicksPerBeat = 96;

    }
}
