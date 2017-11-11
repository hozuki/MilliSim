using JetBrains.Annotations;
using OpenMLTD.MilliSim.Audio;

namespace OpenMLTD.MilliSim.Theater.Extensions {
    internal static class AudioManagerExtensions {

        internal static void StopAll([NotNull] this AudioManager audioManager) {
            foreach (var sound in audioManager.GetLoadedSounds()) {
                sound.Source.Stop();
            }
        }

        internal static void StopAllExcept([NotNull] this AudioManager audioManager, [NotNull] Sound s) {
            foreach (var sound in audioManager.GetLoadedSounds()) {
                if (sound != s) {
                    sound.Source.Stop();
                }
            }
        }

    }
}
