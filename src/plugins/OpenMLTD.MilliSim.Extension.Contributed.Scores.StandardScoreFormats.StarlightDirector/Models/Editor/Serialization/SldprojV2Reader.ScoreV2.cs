using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Beatmap;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Editor.Serialization {
    partial class SldprojV2Reader {

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy), MemberSerialization = MemberSerialization.OptIn)]
        private sealed class ScoreV2 {

            [JsonConstructor]
            public ScoreV2() {
                Bars = new List<BarV2>();
            }

            [JsonProperty]
            public List<BarV2> Bars { get; set; }

            public Score ToScore(Project project, Difficulty difficulty) {
                FixNoteType();
                var score = new Score(project, difficulty);
                foreach (var b in Bars) {
                    var bar = b.ToBar(score);
                    score.Bars.Add(bar);
                }
                return score;
            }

            private void FixNoteType() {
                if (Bars.Count == 0 || !Bars.SelectMany(b => b.Notes).Any()) {
                    return;
                }
                var noteSourceRef = new Dictionary<NoteV2, BarV2>();
                var allNotes = new List<NoteV2>();
                foreach (var b in Bars) {
                    foreach (var n in b.Notes) {
                        noteSourceRef[n] = b;
                        allNotes.Add(n);
                    }
                }
                foreach (var n in allNotes) {
                    if (n.HoldTargetID != 0) {
                        var holdTarget = allNotes.Find(n2 => n2.ID == n.HoldTargetID);
                        if (holdTarget == null) {
                            // WTF happened here?
                            continue;
                        }
                        if (noteSourceRef[n].Index < noteSourceRef[holdTarget].Index ||
                            (noteSourceRef[n].Index == noteSourceRef[holdTarget].Index && n.IndexInGrid < holdTarget.IndexInGrid)) {
                            n.Type = NoteType.Hold;
                        } else {
                            n.Type = NoteType.TapOrFlick;
                        }
                    } else {
                        n.Type = NoteType.TapOrFlick;
                    }
                }
            }

        }

    }
}
