using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Source;

namespace OpenMLTD.MilliSim.Core.Entities.Extending {
    public interface IScoreFormat : IMilliSimPlugin {

        /// <summary>
        /// Gets whether this format can be read as a source score.
        /// If <see cref="CanReadAsSource"/> is <see langword="true"/>, <see cref="CreateCompiler"/> must return a valid compiler, and the <see cref="IScoreReader"/> must correctly implement <see cref="IScoreReader.ReadSourceScore"/>.
        /// </summary>
        bool CanReadAsSource { get; }

        /// <summary>
        /// Gets whether this format can be read as a compiled score.
        /// If <see cref="CanReadAsCompiled"/> is <see langword="true"/>, <see cref="IScoreReader"/> must correctly implement <see cref="IScoreReader.ReadCompiledScore"/>.
        /// </summary>
        bool CanReadAsCompiled { get; }

        /// <summary>
        /// Creates a new <see cref="IScoreReader"/> to read scores.
        /// </summary>
        /// <remarks>You must call <see cref="IScoreReader.Dispose"/> method to dispose the reader.</remarks>
        /// <returns>A score reader.</returns>
        [NotNull]
        IScoreReader CreateReader();

        /// <summary>
        /// Creates a new <see cref="IScoreCompiler"/> to compile a <see cref="SourceScore"/> to a <see cref="RuntimeScore"/>.
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
