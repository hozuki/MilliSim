using UnityStudio.Serialization;
using UnityStudio.Serialization.Naming;

namespace OpenMLTD.MilliSim.Extension.Scores.StandardScoreFormats.Mltd.Serialization {
    [MonoBehavior(NamingConventionType = typeof(CamelCaseNamingConvetion))]
    internal sealed class PolyPointData {

        [MonoBehaviorProperty(Name = "subtick")]
        internal int SubTick { get; set; }

        [MonoBehaviorProperty(Name = "posx")]
        internal float PositionX { get; set; }

    }
}
