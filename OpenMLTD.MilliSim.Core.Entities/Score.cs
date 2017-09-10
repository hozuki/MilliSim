using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core.Entities {
    [Serializable]
    [DataContract]
    public sealed class Score {

        [CanBeNull, ItemNotNull]
        [DataMember(Name = "notes")]
        public Note[] Notes { get; set; }

        [CanBeNull, ItemNotNull]
        [DataMember(Name = "conductors")]
        public Conductor[] Conductors { get; set; }

        [CanBeNull]
        [DataMember(Name = "scoreSpeeds")]
        public float[] ScoreSpeeds { get; set; }

        [CanBeNull]
        [DataMember(Name = "judgeRanges")]
        public float[] JudgeRanges { get; set; }

        [DataMember(Name = "musicOffset")]
        public float MusicOffset { get; set; }

    }
}
