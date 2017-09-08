using System.IO;

namespace OpenMLTD.MilliSim.Core.Entities.Extending {
    public interface IScoreReader {

        bool TryRead(Stream stream, string fileName, out Score score);

        /// <summary>
        /// Tests if this reader supports a specific file type.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>Whether this reader may support the file.</returns>
        bool SupportsFileType(string fileName);

        /// <summary>
        /// Gets the description of this reader.
        /// </summary>
        string Description { get; }

    }
}
