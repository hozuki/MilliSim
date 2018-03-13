using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Audio;
using OpenMLTD.MilliSim.Audio.Extending;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents {
    public sealed class SfxController : BaseGameComponent {

        public SfxController([NotNull] BaseGame game, [NotNull] IBaseGameComponentContainer parent)
            : base(game, parent) {
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            var theaterDays = Game.ToBaseGame();
            var store = ConfigurationStore;
            var config = store.Get<SfxControllerConfig>();

            var audioManager = theaterDays.AudioManager;

            var sfxSounds = new List<Sound>();
            PreloadAudio(audioManager, config.Data.Sfx.Tap, sfxSounds);
            PreloadAudio(audioManager, config.Data.Sfx.Hold, sfxSounds);
            PreloadAudio(audioManager, config.Data.Sfx.Flick, sfxSounds);
            PreloadAudio(audioManager, config.Data.Sfx.Slide, sfxSounds);
            PreloadAudio(audioManager, config.Data.Sfx.HoldHold, sfxSounds);
            PreloadAudio(audioManager, config.Data.Sfx.SlideHold, sfxSounds);
            PreloadAudio(audioManager, config.Data.Sfx.HoldEnd, sfxSounds);
            PreloadAudio(audioManager, config.Data.Sfx.SlideEnd, sfxSounds);
            PreloadAudio(audioManager, config.Data.Sfx.Special, sfxSounds);
            PreloadAudio(audioManager, config.Data.Sfx.SpecialHold, sfxSounds);
            PreloadAudio(audioManager, config.Data.Sfx.SpecialEnd, sfxSounds);

            if (config.Data.Sfx.Shouts != null) {
                foreach (var shoutPath in config.Data.Sfx.Shouts) {
                    PreloadAudio(audioManager, shoutPath, sfxSounds);
                }
            }

            foreach (var sound in sfxSounds) {
                sound.Source.Volume = config.Data.SfxVolume.Value;
            }
        }

        private void PreloadAudio(AudioManager audioManager, string fileName, List<Sound> sounds) {
            var theaterDays = Game.ToBaseGame();
            var debugOverlay = theaterDays.FindSingleElement<DebugOverlay>();
            var pluginManager = theaterDays.PluginManager;
            var format = GetFormatForFile(pluginManager, fileName);

            if (format != null) {
                var sound = audioManager.LoadSound(fileName, format);
                sounds.Add(sound);
            } else {
                if (debugOverlay != null) {
                    debugOverlay.AddLine($"Audio file '{fileName}' is not supported.");
                }
            }
        }

        private void PreloadAudio(AudioManager audioManager, SfxControllerConfig.NoteSfxGroup group, List<Sound> sounds) {
            PreloadAudio(audioManager, group.Perfect, sounds);
            PreloadAudio(audioManager, group.Great, sounds);
            PreloadAudio(audioManager, group.Nice, sounds);
            PreloadAudio(audioManager, group.Bad, sounds);
            PreloadAudio(audioManager, group.Miss, sounds);
        }

        [CanBeNull]
        private static IAudioFormat GetFormatForFile(BasePluginManager pluginManager, string fileName) {
            return pluginManager.GetPluginsOfType<IAudioFormat>().FirstOrDefault(format => format.SupportsFileType(fileName));
        }

    }
}
