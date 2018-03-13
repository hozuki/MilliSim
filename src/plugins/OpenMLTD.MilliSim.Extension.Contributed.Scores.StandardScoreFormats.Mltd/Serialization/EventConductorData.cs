using UnityStudio.Serialization;
using UnityStudio.Serialization.Naming;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.Mltd.Serialization {
    [MonoBehaviour(NamingConventionType = typeof(CamelCaseNamingConvention))]
    internal sealed class EventConductorData {

        [MonoBehaviourProperty(Name = "absTime", ConverterType = typeof(DoubleToSingleConverter))]
        internal float AbsoluteTime { get; set; }

        internal bool Selected { get; set; }

        internal long Tick { get; set; }

        internal int Measure { get; set; }

        internal int Beat { get; set; }

        internal int Track { get; set; }

        [MonoBehaviourProperty(ConverterType = typeof(DoubleToSingleConverter))]
        internal float Tempo { get; set; }

        [MonoBehaviourProperty(Name = "tsigNumerator")]
        internal int SignatureNumerator { get; set; }

        [MonoBehaviourProperty(Name = "tsigDenominator")]
        internal int SignatureDenominator { get; set; }

        internal string Marker { get; set; }

    }
}
