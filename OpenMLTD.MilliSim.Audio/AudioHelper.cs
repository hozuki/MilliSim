using NAudio.Wave;

namespace OpenMLTD.MilliSim.Audio {
    internal static class AudioHelper {

        internal static bool NeedsFormatConversionFrom(WaveFormat sourceFormat, WaveFormat requiredFormat) {
            return sourceFormat.SampleRate != requiredFormat.SampleRate ||
                   sourceFormat.BitsPerSample != requiredFormat.BitsPerSample ||
                   sourceFormat.Channels != requiredFormat.Channels ||
                   sourceFormat.Encoding != requiredFormat.Encoding;
        }

    }
}
