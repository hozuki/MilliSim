using System;
using System.Globalization;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Beatmap {
    public sealed class NoteExtraParams {

        internal NoteExtraParams(Note note) {
            Note = note;
        }

        public double NewBpm { get; set; } = 120;

        public string GetDataString() {
            switch (Note.Basic.Type) {
                case NoteType.VariantBpm:
                    return NewBpm.ToString(CultureInfo.InvariantCulture);
                default:
                    throw new ArgumentOutOfRangeException(nameof(Note.Basic.Type));
            }
        }

        public void UpdateByDataString(string s) {
            var note = Note;
            switch (note.Basic.Type) {
                case NoteType.VariantBpm:
                    NewBpm = double.Parse(s);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(note.Basic.Type));
            }
        }

        public static NoteExtraParams FromDataString(string str, Note note) {
            if (string.IsNullOrEmpty(str)) {
                return null;
            }
            var p = new NoteExtraParams(note);
            switch (note.Basic.Type) {
                case NoteType.VariantBpm:
                    p.NewBpm = double.Parse(str);
                    break;
                default:
                    break;
            }
            return p;
        }

        public Note Note { get; }

    }
}
