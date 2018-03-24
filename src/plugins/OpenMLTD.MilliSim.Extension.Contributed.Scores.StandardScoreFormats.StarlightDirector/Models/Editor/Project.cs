using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Beatmap;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Editor {
    public sealed class Project {

        public Project() {
            var scores = new Dictionary<Difficulty, Score>();

            _scores = scores;

            for (var i = Difficulty.Debut; i <= Difficulty.MasterPlus; ++i) {
                scores.Add(i, new Score(this, i));
            }

            Settings = ProjectSettings.CreateDefault();
        }

        [NotNull]
        public static Project CreateWithVersion(int version) {
            var project = new Project();

            project.Version = version;

            return project;
        }

        [CanBeNull]
        public Score GetScore(Difficulty difficulty) {
            return GetScore(difficulty, false);
        }

        [CanBeNull]
        public Score GetScore(Difficulty difficulty, bool throwIfNotFound) {
            if (throwIfNotFound) {
                return _scores[difficulty];
            } else {
                return _scores.ContainsKey(difficulty) ? _scores[difficulty] : null;
            }
        }

        [NotNull]
        public ProjectSettings Settings { get; }

        [NotNull]
        public string MusicFileName { get; set; } = string.Empty;

        public bool HasMusic => !string.IsNullOrEmpty(MusicFileName);

        public int Version { get; internal set; }

        [NotNull]
        public HashSet<Guid> UsedNoteIDs { get; } = new HashSet<Guid>();

        internal void SetScore(Difficulty difficulty, [NotNull] Score score) {
            switch (difficulty) {
                case Difficulty.Debut:
                case Difficulty.Regular:
                case Difficulty.Pro:
                case Difficulty.Master:
                case Difficulty.MasterPlus:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(difficulty), "Difficulty is invalid.");
            }

            _scores[difficulty] = score;
        }

        [NotNull]
        private readonly Dictionary<Difficulty, Score> _scores;

    }
}
