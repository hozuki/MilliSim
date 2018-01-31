using JetBrains.Annotations;
using NAudio.Wave;

namespace OpenMLTD.MilliSim.Audio {
    internal static class AudioHelper {

        internal static bool NeedsFormatConversionFrom([NotNull] WaveFormat sourceFormat, [NotNull] WaveFormat requiredFormat) {
            return sourceFormat.SampleRate != requiredFormat.SampleRate ||
                   sourceFormat.BitsPerSample != requiredFormat.BitsPerSample ||
                   sourceFormat.Channels != requiredFormat.Channels ||
                   sourceFormat.Encoding != requiredFormat.Encoding;
        }

        internal static float SampleOffsetToTimeOffset(int sampleOffset, int sampleRate) {
            return (float)sampleOffset / sampleRate;
        }

    }
}
