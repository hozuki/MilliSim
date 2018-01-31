using System;
using JetBrains.Annotations;
using NAudio.Wave;

namespace OpenMLTD.MilliSim.Audio.Extensions {
    public static class AudioSourceExtensions {

        public static TimeSpan GetCurrentTime([NotNull] this AudioSource audioSource, int sampleRate) {
            var sampleOffset = audioSource.SampleOffset;
            var timeOffset = AudioHelper.SampleOffsetToTimeOffset(sampleOffset, sampleRate);

            return TimeSpan.FromSeconds(timeOffset);
        }

        public static TimeSpan GetCurrentTime([NotNull] this AudioSource audioSource, [NotNull] WaveFormat format) {
            return GetCurrentTime(audioSource, format.SampleRate);
        }

    }
}
