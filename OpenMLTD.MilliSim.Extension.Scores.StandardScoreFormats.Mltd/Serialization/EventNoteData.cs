using UnityStudio.Serialization;
using UnityStudio.Serialization.Naming;

namespace OpenMLTD.MilliSim.Extension.Scores.StandardScoreFormats.Mltd.Serialization {
    [MonoBehavior(NamingConventionType = typeof(CamelCaseNamingConvention))]
    internal sealed class EventNoteData {

        [MonoBehaviorProperty(Name = "absTime")]
        internal double AbsoluteTime { get; set; }

        internal bool Selected { get; set; }

        internal long Tick { get; set; }

        internal int Measure { get; set; }

        internal int Beat { get; set; }

        internal int Track { get; set; }

        internal int Type { get; set; }

        [MonoBehaviorProperty(Name = "startPosx")]
        internal float StartPositionX { get; set; }

        [MonoBehaviorProperty(Name = "endPosx")]
        internal float EndPositionX { get; set; }

        internal float Speed { get; set; }

        internal int Duration { get; set; }

        [MonoBehaviorProperty(Name = "poly")]
        internal PolyPointData[] Polypoints { get; set; }

        internal int EndType { get; set; }

        internal double LeadTime { get; set; }

    }
}
