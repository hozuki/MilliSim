using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core.Entities.Runtime;

namespace OpenMLTD.MilliSim.Core.Entities.Extending {
    public interface IScoreFormat : IMilliSimPlugin {

        /// <summary>
        /// Creates a new <see cref="IScoreReader"/> to read scores.
        /// </summary>
        /// <remarks>You must call <see cref="IScoreReader.Dispose"/> method to dispose the reader.</remarks>
        /// <returns>A score reader.</returns>
        [NotNull]
        IScoreReader CreateReader();

        /// <summary>
        /// Creates a new <see cref="IScoreCompiler"/> to compile a <see cref="Score"/> to a <see cref="RuntimeScore"/>.
        /// </summary>
        /// <remarks>You must call <see cref="IScoreCompiler.Dispose"/> method to dispose the compiler.</remarks>
        /// <returns>A score compiler.</returns>
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
        string FormatDescription { get; }

    }
}
