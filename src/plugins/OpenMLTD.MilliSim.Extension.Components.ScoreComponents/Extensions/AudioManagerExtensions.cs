using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Audio;
using OpenMLTD.MilliSim.Plugin;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Extensions {
    internal static class AudioManagerExtensions {

        internal static void Play([NotNull] this AudioManager audioManager, [CanBeNull] string fileName, [NotNull, ItemNotNull] IEnumerable<IAudioFormat> formats) {
            if (fileName == null) {
                return;
            }

            var format = GetFormatForFile(formats, fileName);
            if (format != null) {
                audioManager.LoadSound(fileName, format).Source.Play();
            }
        }

        internal static void PlayLooped([NotNull] this AudioManager audioManager, [CanBeNull] string fileName, [NotNull, ItemNotNull] IEnumerable<IAudioFormat> formats, [NotNull] object state) {
            if (fileName == null) {
                return;
            }

            var format = GetFormatForFile(formats, fileName);
            if (format != null) {
                var sound = audioManager.LoadSound(fileName, format);
                GetDictionary(audioManager)[state] = sound;
                sound.Source.PlayLooped();
            }
        }

        internal static void StopLooped([NotNull] this AudioManager audioManager, [NotNull] object state) {
            var dict = GetDictionary(audioManager);

            if (!dict.ContainsKey(state)) {
                return;
            }

            var sound = dict[state];
            sound.Source.Stop();

            dict.Remove(state);
        }

        [CanBeNull]
        private static IAudioFormat GetFormatForFile([NotNull, ItemNotNull] IEnumerable<IAudioFormat> formats, [NotNull] string fileName) {
            return formats.FirstOrDefault(format => format.SupportsFileType(fileName));
        }

        private static Dictionary<object, Sound> GetDictionary([NotNull] AudioManager audioManager) {
            if (LoopDict.ContainsKey(audioManager)) {
                return LoopDict[audioManager];
            } else {
                var dict = new Dictionary<object, Sound>();
                LoopDict[audioManager] = dict;
                return dict;
            }
        }

        private static readonly Dictionary<AudioManager, Dictionary<object, Sound>> LoopDict = new Dictionary<AudioManager, Dictionary<object, Sound>>();

    }
}
