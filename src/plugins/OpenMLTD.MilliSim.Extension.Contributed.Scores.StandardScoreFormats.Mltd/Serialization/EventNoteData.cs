using UnityStudio.Serialization;
using UnityStudio.Serialization.Naming;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.Mltd.Serialization {
    [MonoBehaviour(NamingConventionType = typeof(CamelCaseNamingConvention))]
    internal sealed class EventNoteData {

        [MonoBehaviourProperty(Name = "absTime", ConverterType = typeof(DoubleToSingleConverter))]
        internal float AbsoluteTime { get; set; }

        internal bool Selected { get; set; }

        internal long Tick { get; set; }

        internal int Measure { get; set; }

        internal int Beat { get; set; }

        internal int Track { get; set; }

        internal int Type { get; set; }

        [MonoBehaviourProperty(Name = "startPosx")]
        internal float StartPositionX { get; set; }

        [MonoBehaviourProperty(Name = "endPosx")]
        internal float EndPositionX { get; set; }

        internal float Speed { get; set; }

        internal int Duration { get; set; }

        [MonoBehaviourProperty(Name = "poly")]
        internal PolyPointData[] Polypoints { get; set; }

        internal int EndType { get; set; }

        [MonoBehaviourProperty(ConverterType = typeof(DoubleToSingleConverter))]
        internal float LeadTime { get; set; }

    }
}
