using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core.Entities.Extending {
    public interface IScoreFormat {

        [NotNull]
        IScoreReader CreateReader();

        [NotNull]
        IScoreCompiler CreateCompiler();

        /// <summary>
        /// Tests if this format supports a specific file type.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>Whether this format may support the file.</returns>
        bool SupportsFileType([NotNull] string fileName);

        /// <summary>
        /// Gets the description of this format.
        /// </summary>
        [NotNull]
        string Description { get; }

    }
}
