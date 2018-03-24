using System.Collections.Generic;
using System.Linq;
using OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Editor;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Beatmap {
    public sealed class Score {

        public Score(Project project, Difficulty difficulty) {
            Project = project;
            Difficulty = difficulty;
        }

        public Difficulty Difficulty { get; internal set; }

        public Project Project { get; }

        public List<Bar> Bars { get; } = new List<Bar>();

        public IReadOnlyList<Note> GetAllNotes() {
            return Bars.SelectMany(measure => measure.Notes).ToArray();
        }

        public bool HasAnyNote {
            get {
                return Bars.Count != 0 && Bars.Any(bar => bar.Notes.Count > 0);
            }
        }

        public bool HasAnyBar => Bars.Count != 0;

    }
}
