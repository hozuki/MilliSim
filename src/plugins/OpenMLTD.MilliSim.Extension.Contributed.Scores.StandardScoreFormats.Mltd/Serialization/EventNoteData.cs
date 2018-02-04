using UnityStudio.Serialization;
using UnityStudio.Serialization.Naming;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.Mltd.Serialization {
    [MonoBehavior(NamingConventionType = typeof(CamelCaseNamingConvention))]
    internal sealed class EventNoteData {

        [MonoBehaviorProperty(Name = "absTime", ConverterType = typeof(DoubleToSingleConverter))]
        internal float AbsoluteTime { get; set; }

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

        [MonoBehaviorProperty(ConverterType = typeof(DoubleToSingleConverter))]
        internal float LeadTime { get; set; }

    }
}
