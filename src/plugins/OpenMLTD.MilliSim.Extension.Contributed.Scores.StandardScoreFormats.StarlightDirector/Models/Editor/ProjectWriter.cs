using JetBrains.Annotations;
using OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Editor.Serialization;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Editor {
    public abstract class ProjectWriter {

        /// <summary>
        /// Writes a <see cref="Project"/> to a persistence file.
        /// </summary>
        /// <param name="project">The project to save.</param>
        /// <param name="fileName">Path to the save file.</param>
        public abstract void WriteProject([NotNull] Project project, [NotNull] string fileName);

        [NotNull]
        public static ProjectWriter CreateLatest() {
            return new SldprojV4Writer();
        }

    }
}
