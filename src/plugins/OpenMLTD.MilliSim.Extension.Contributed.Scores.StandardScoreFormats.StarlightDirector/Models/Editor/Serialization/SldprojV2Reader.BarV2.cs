using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Beatmap;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Editor.Serialization {
    partial class SldprojV2Reader {

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy), MemberSerialization = MemberSerialization.OptIn)]
        private sealed class BarV2 {

            [JsonConstructor]
            public BarV2() {
                Notes = new List<NoteV2>();
            }

            [JsonProperty]
            public List<NoteV2> Notes { get; }

            [JsonProperty]
            public BarParamsV2 Params { get; set; }

            [JsonProperty]
            public int Index { get; set; }

            public Bar ToBar(Score score) {
                var bar = new Bar(score, Index);
                if (Params != null) {
                    bar.Params = new BarParams {
                        UserDefinedBpm = Params.UserDefinedBpm,
                        UserDefinedGridPerSignature = Params.UserDefinedGridPerSignature,
                        UserDefinedSignature = Params.UserDefinedSignature
                    };
                }
                foreach (var n in Notes) {
                    var note = n.ToNote(bar);
                    bar.Notes.Add(note);
                }
                return bar;
            }

        }

    }
}
