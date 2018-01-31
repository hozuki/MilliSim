using JetBrains.Annotations;
using NAudio.Wave;

namespace OpenMLTD.MilliSim.Plugin {
    public interface IAudioFormat : IMilliSimPlugin {

        /// <summary>
        /// Reads an audio file and creates a <see cref="WaveStream"/> to play.
        /// </summary>
        /// <param name="fileName">The path of the audio file to read.</param>
        /// <remarks>You should call <see cref="WaveStream.Dispose"/> when you finished using the wave stream.</remarks>
        /// <returns>A wave stream.</returns>
        [NotNull]
        WaveStream Read([NotNull] string fileName);

        /// <summary>
        /// Checks if this <see cref="IAudioFormat"/> supports specified file.
        /// </summary>
        /// <param name="fileName">The name of the file to check.</param>
        /// <returns><see langword="true"/> if this <see cref="IAudioFormat"/> supports the file. Otherwise, <see langword="false"/>.</returns>
        bool SupportsFileType([NotNull] string fileName);

        /// <summary>
        /// Gets the description of the audio format.
        /// </summary>
        [NotNull]
        string FormatDescription { get; }

    }
}
