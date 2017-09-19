using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Source;

namespace OpenMLTD.MilliSim.Core.Entities.Extending {
    /// <summary>
    /// Represents a score format.
    /// </summary>
    public interface IScoreFormat : IMilliSimPlugin {

        /// <summary>
        /// Gets whether this format can be read as a source score.
        /// If <see cref="CanReadAsSource"/> is <see langword="true"/>, <see cref="CanBeCompiled"/> must be <see langword="true"/>, and the <see cref="IScoreReader"/> must correctly implement <see cref="IScoreReader.ReadSourceScore"/>.
        /// </summary>
        bool CanReadAsSource { get; }

        /// <summary>
        /// Gets whether this format can be read as a compiled score.
        /// If <see cref="CanReadAsCompiled"/> is <see langword="true"/>, <see cref="IScoreReader"/> must correctly implement <see cref="IScoreReader.ReadCompiledScore"/>.
        /// </summary>
        bool CanReadAsCompiled { get; }

        /// <summary>
        /// Gets whether this format can be compiled from <see cref="SourceScore"/> to <see cref="RuntimeScore"/>.
        /// </summary>
        bool CanBeCompiled { get; }

        /// <summary>
        /// Gets whether this format can write a source score to a file.
        /// If <see cref="CanWriteSource"/> is <see langword="true"/>, <see cref="IScoreWriter"/> must correctly implement <see cref="IScoreWriter.WriteSourceScore"/>.
        /// </summary>
        bool CanWriteSource { get; }

        /// <summary>
        /// Gets whether this format can write a source score to a file.
        /// If <see cref="CanWriteCompiled"/> is <see langword="true"/>, <see cref="IScoreWriter"/> must correctly implement <see cref="IScoreWriter.WriteCompiledScore"/>.
        /// </summary>
        bool CanWriteCompiled { get; }

        /// <summary>
        /// Creates a new <see cref="IScoreReader"/> to read scores.
        /// </summary>
        /// <remarks>You must call <see cref="IScoreReader.Dispose"/> method to dispose the reader.</remarks>
        /// <exception cref="NotSupportedException">This format does not support reading scores.</exception>
        /// <returns>A score reader.</returns>
        [NotNull]
        IScoreReader CreateReader();

        /// <summary>
        /// Creates a new <see cref="IScoreWriter"/> to write scores.
        /// </summary>
        /// <remarks>You must call <see cref="IScoreWriter.Dispose"/> method to dispose the writer.</remarks>
        /// <exception cref="NotSupportedException">This format does not support writing scores.</exception>
        /// <returns>A score writer.</returns>
        [NotNull]
        IScoreWriter CreateWriter();

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
        bool SupportsReadingFileType([NotNull] string fileName);

        /// <summary>
        /// Gets a list of file extensions (with ".") that can be recognized by the <see cref="IScoreReader"/> created from this <see cref="IScoreFormat"/>.
        /// </summary>
        /// <remarks>Useful when displaying file dialogs.</remarks>
        [NotNull]
        IReadOnlyList<string> SupportedReadExtensions { get; }

        /// <summary>
        /// Gets a list of file extensions (with ".") that can be recognized by the <see cref="IScoreWriter"/> created from this <see cref="IScoreFormat"/>.
        /// </summary>
        /// <remarks>Useful when displaying file dialogs.</remarks>
        [NotNull]
        IReadOnlyList<string> SupportedWriteExtensions { get; }

        /// <summary>
        /// Gets the description of this format.
        /// </summary>
        [NotNull]
        string FormatDescription { get; }

    }
}
