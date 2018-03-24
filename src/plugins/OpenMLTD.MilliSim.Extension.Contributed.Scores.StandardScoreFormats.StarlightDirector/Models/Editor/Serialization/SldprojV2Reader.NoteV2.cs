using System;
using Newtonsoft.Json;
using OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Beatmap;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Editor.Serialization {
    partial class SldprojV2Reader {

        private sealed class NoteV2 {

            [JsonProperty("positionInGrid")]
            public int IndexInGrid { get; set; }

            [JsonProperty]
            public int ID { get; set; }

            [JsonProperty]
            public NoteType Type { get; set; }

            [JsonProperty]
            public NotePosition StartPosition { get; set; }

            [JsonProperty]
            public NotePosition FinishPosition { get; set; }

            [JsonProperty]
            public NoteFlickType FlickType { get; set; }

            [JsonProperty]
            public int PrevFlickNoteID { get; set; }

            [JsonProperty]
            public int NextFlickNoteID { get; set; }

            [Obsolete("This property is provided for forward compatibility only.")]
            [JsonProperty]
            public int SyncTargetID { get; set; }

            [JsonProperty]
            public int HoldTargetID { get; set; }

            public Note ToNote(Bar bar) {
                var guid = StarlightID.GetGuidFromInt32(ID);
                var note = new Note(bar, guid);
                var b = note.Basic;
                b.IndexInGrid = IndexInGrid;
                b.Type = Type;
                b.StartPosition = StartPosition;
                b.FinishPosition = FinishPosition;
                b.FlickType = FlickType;
                var t = note.Temporary;
                t.PrevFlickNoteID = StarlightID.GetGuidFromInt32(PrevFlickNoteID);
                t.NextFlickNoteID = StarlightID.GetGuidFromInt32(NextFlickNoteID);
                t.HoldTargetID = StarlightID.GetGuidFromInt32(HoldTargetID);
                return note;
            }

        }

    }
}
