using UnityStudio.Serialization;
using UnityStudio.Serialization.Naming;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.Mltd.Serialization {
    [MonoBehaviour(NamingConventionType = typeof(CamelCaseNamingConvention))]
    internal sealed class ScoreObject {

        [MonoBehaviourProperty(Name = "evts")]
        internal EventNoteData[] NoteEvents { get; set; }

        [MonoBehaviourProperty(Name = "ct")]
        internal EventConductorData[] ConductorEvents { get; set; }

        internal float[] JudgeRange { get; set; }

        internal float[] ScoreSpeed { get; set; }

        [MonoBehaviourProperty(Name = "BGM_offset")]
        internal float BgmOffset { get; set; }

    }
}
