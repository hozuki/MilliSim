using UnityStudio.Serialization;
using UnityStudio.Serialization.Naming;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.Mltd.Serialization {
    [MonoBehaviour(NamingConventionType = typeof(CamelCaseNamingConvention))]
    internal sealed class PolyPointData {

        [MonoBehaviourProperty(Name = "subtick")]
        internal int SubTick { get; set; }

        [MonoBehaviourProperty(Name = "posx")]
        internal float PositionX { get; set; }

    }
}
