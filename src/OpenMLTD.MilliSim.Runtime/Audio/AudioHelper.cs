using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using NAudio.Wave;

namespace OpenMLTD.MilliSim.Audio {
    /// <summary>
    /// Audio helper functions.
    /// </summary>
    internal static class AudioHelper {

        /// <summary>
        /// Checks if format conversion from source is needed.
        /// </summary>
        /// <param name="sourceFormat">The format of wave data source.</param>
        /// <param name="requiredFormat">Required wave format.</param>
        /// <returns><see langword="true"/> if conversion is needed, otherwise <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool NeedsFormatConversionFrom([NotNull] WaveFormat sourceFormat, [NotNull] WaveFormat requiredFormat) {
            return sourceFormat.SampleRate != requiredFormat.SampleRate ||
                   sourceFormat.BitsPerSample != requiredFormat.BitsPerSample ||
                   sourceFormat.Channels != requiredFormat.Channels ||
                   sourceFormat.Encoding != requiredFormat.Encoding;
        }

        /// <summary>
        /// Calculates time offset, from sample offset.
        /// </summary>
        /// <param name="sampleOffset">Sample offset.</param>
        /// <param name="sampleRate">Sample rate, in Hz.</param>
        /// <returns>Time offset, in seconds.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float SampleOffsetToTimeOffset(int sampleOffset, int sampleRate) {
            return (float)sampleOffset / sampleRate;
        }

    }
}
