using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core.Entities {
    public sealed class Score {

        [CanBeNull, ItemNotNull]
        public IReadOnlyList<Note> Notes { get; set; }

        [CanBeNull, ItemNotNull]
        public IReadOnlyList<Conductor> Conductors { get; set; }

        [CanBeNull]
        public IReadOnlyList<float> ScoreSpeeds { get; set; }

        [CanBeNull]
        public IReadOnlyList<float> JudgeRanges { get; set; }

        public float MusicOffset { get; set; }

    }
}
