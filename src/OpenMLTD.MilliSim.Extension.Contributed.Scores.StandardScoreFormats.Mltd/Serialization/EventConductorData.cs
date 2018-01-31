using UnityStudio.Serialization;
using UnityStudio.Serialization.Naming;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.Mltd.Serialization {
    [MonoBehavior(NamingConventionType = typeof(CamelCaseNamingConvention))]
    internal sealed class EventConductorData {

        [MonoBehaviorProperty(Name = "absTime")]
        internal double AbsoluteTime { get; set; }

        internal bool Selected { get; set; }

        internal long Tick { get; set; }

        internal int Measure { get; set; }

        internal int Beat { get; set; }

        internal int Track { get; set; }

        internal double Tempo { get; set; }

        [MonoBehaviorProperty(Name = "tsigNumerator")]
        internal int SignatureNumerator { get; set; }

        [MonoBehaviorProperty(Name = "tsigDenominator")]
        internal int SignatureDenominator { get; set; }

        internal string Marker { get; set; }

    }
}
