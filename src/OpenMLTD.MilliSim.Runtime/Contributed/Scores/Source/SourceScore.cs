using JetBrains.Annotations;
using OpenMLTD.MilliSim.Plugin;

namespace OpenMLTD.MilliSim.Contributed.Scores.Source {
    /// <summary>
    /// Source score.
    /// </summary>
    public class SourceScore {

        /// <summary>
        /// Notes. May include gaming notes and special notes.
        /// </summary>
        [NotNull, ItemNotNull]
        public SourceNote[] Notes { get; set; }

        /// <summary>
        /// Score BPM indicators. Every score must have at least 1 <see cref="Conductor"/> at the start to indicate the starting BPM.
        /// </summary>
        [NotNull, ItemNotNull]
        public Conductor[] Conductors { get; set; }

        /// <summary>
        /// Offset of this score, relative to the music, in seconds.
        /// </summary>
        public double MusicOffset { get; set; }

        /// <summary>
        /// Total number  of tracks in this score.
        /// </summary>
        public int TrackCount { get; set; }

        /// <summary>
        /// The index of this score in <see cref="IScoreFormat"/>. An <see cref="IScoreFormat"/> may contain multiple scores.
        /// </summary>
        public int ScoreIndex { get; set; }

    }
}
