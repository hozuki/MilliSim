using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Beatmap;
using OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Beatmap.Extensions;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Editor.Serialization {
    public sealed partial class SldprojV2Reader : ProjectReader {

        public override Project ReadProject(string fileName, object state) {
            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Exists) {
                throw new IOException($"File '{fileName}' does not exist.");
            }
            if (fileInfo.Length == 0) {
                throw new IOException($"File '{fileName}' is empty.");
            }
            var builder = new SQLiteConnectionStringBuilder {
                DataSource = fileName
            };
            Project project;
            using (var db = new SQLiteConnection(builder.ToString())) {
                db.Open();
                project = ReadProject(db);
                db.Close();
            }
            project.Version = ProjectVersion.Current;
            return project;
        }

        private static Project ReadProject(SQLiteConnection db) {
            var project = new Project();
            SQLiteCommand getValues = null;

            // Main
            var mainValues = SQLiteHelper.GetValues(db, SldprojDbNames.Table_Main, ref getValues);
            project.MusicFileName = mainValues[SldprojDbNames.Field_MusicFileName];
            //project.Version = mainValues[Names.Field_Version];

            // Scores
            var scoreValues = SQLiteHelper.GetValues(db, SldprojDbNames.Table_Scores, ref getValues);
            var jsonSerializer = JsonSerializer.CreateDefault();
            foreach (var difficulty in Difficulties) {
                var indexString = ((int)difficulty).ToString("00");
                var scoreJson = scoreValues[indexString];
                ScoreV2 scoreV2;
                var scoreJsonBytes = Encoding.ASCII.GetBytes(scoreJson);
                using (var memoryStream = new MemoryStream(scoreJsonBytes)) {
                    using (var reader = new StreamReader(memoryStream)) {
                        using (var jsonReader = new JsonTextReader(reader)) {
                            scoreV2 = jsonSerializer.Deserialize<ScoreV2>(jsonReader);
                        }
                    }
                }
                var score = scoreV2.ToScore(project, difficulty);
                ResolveReferences(score);
                FixSyncNotes(score);
                project.SetScore(difficulty, score);
            }

            // Score settings
            var scoreSettingsValues = SQLiteHelper.GetValues(db, SldprojDbNames.Table_ScoreSettings, ref getValues);
            var settings = project.Settings;
            settings.BeatPerMinute = double.Parse(scoreSettingsValues[SldprojDbNames.Field_GlobalBpm]);
            settings.StartTimeOffset = double.Parse(scoreSettingsValues[SldprojDbNames.Field_StartTimeOffset]);
            settings.GridPerSignature = int.Parse(scoreSettingsValues[SldprojDbNames.Field_GlobalGridPerSignature]);
            settings.Signature = int.Parse(scoreSettingsValues[SldprojDbNames.Field_GlobalSignature]);

            // Cleanup
            getValues.Dispose();

            return project;
        }

        private static void ResolveReferences(Score score) {
            if (score.Bars == null) {
                return;
            }
            var allNotes = score.Bars.SelectMany(bar => bar.Notes).ToArray();
            foreach (var note in allNotes) {
                if (!note.Helper.IsGaming) {
                    continue;
                }
                if (note.Temporary.NextFlickNoteID != Guid.Empty) {
                    note.Editor.NextFlick = score.FindNoteByID(note.Temporary.NextFlickNoteID);
                }
                if (note.Temporary.PrevFlickNoteID != Guid.Empty) {
                    note.Editor.PrevFlick = score.FindNoteByID(note.Temporary.PrevFlickNoteID);
                }
                if (note.Temporary.HoldTargetID != Guid.Empty) {
                    note.Editor.HoldPair = score.FindNoteByID(note.Temporary.HoldTargetID);
                }
            }
        }

        private static void FixSyncNotes(Score score) {
            foreach (var bar in score.Bars) {
                var gridIndexGroups =
                    from n in bar.Notes
                    where n.Helper.IsGaming
                    group n by n.Basic.IndexInGrid
                    into g
                    select g;
                foreach (var group in gridIndexGroups) {
                    var sortedNotesInGroup =
                        from n in @group
                        orderby n.Basic.FinishPosition
                        select n;
                    Note prev = null;
                    foreach (var note in sortedNotesInGroup) {
                        NoteHelper.MakeSync(prev, note);
                        prev = note;
                    }
                    NoteHelper.MakeSync(prev, null);
                }
            }
        }

        private static readonly Difficulty[] Difficulties = { Difficulty.Debut, Difficulty.Regular, Difficulty.Pro, Difficulty.Master, Difficulty.MasterPlus };

    }
}
