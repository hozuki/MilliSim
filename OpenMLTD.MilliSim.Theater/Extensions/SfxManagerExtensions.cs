using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Audio;
using OpenMLTD.MilliSim.Audio.Extending;

namespace OpenMLTD.MilliSim.Theater.Extensions {
    internal static class SfxManagerExtensions {

        internal static void Play([NotNull] this SfxManager sfx, [CanBeNull] string fileName, [NotNull, ItemNotNull] IEnumerable<IAudioFormat> formats) {
            if (fileName == null) {
                return;
            }

            var format = GetFormatForFile(formats, fileName);
            if (format != null) {
                sfx.Play(fileName, format);
            }
        }

        internal static void PlayLooped([NotNull] this SfxManager sfx, [CanBeNull] string fileName, [NotNull, ItemNotNull] IEnumerable<IAudioFormat> formats, [NotNull] object state) {
            if (fileName == null) {
                return;
            }

            var format = GetFormatForFile(formats, fileName);
            if (format != null) {
                sfx.PlayLooped(fileName, format, state);
            }
        }

        [CanBeNull]
        private static IAudioFormat GetFormatForFile(IEnumerable<IAudioFormat> formats, string fileName) {
            return formats.FirstOrDefault(format => format.SupportsFileType(fileName));
        }

    }
}
