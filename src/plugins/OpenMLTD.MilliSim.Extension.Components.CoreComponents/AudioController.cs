using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Audio;
using OpenMLTD.MilliSim.Audio.Extending;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public sealed class AudioController : BaseGameComponent {

        public AudioController([NotNull] BaseGame game, [NotNull] IBaseGameComponentContainer parent)
            : base(game, parent) {
        }

        [CanBeNull]
        public Sound Music { get; private set; }

        protected override void OnInitialize() {
            base.OnInitialize();

            var theaterDays = Game.ToBaseGame();
            var store = ConfigurationStore;
            var config = store.Get<AudioControllerConfig>();

            var audioManager = theaterDays.AudioManager;

            if (config.Data.BackgroundMusic != null && File.Exists(config.Data.BackgroundMusic)) {
                var format = GetFormatForFile(theaterDays.PluginManager, config.Data.BackgroundMusic);
                if (format != null) {
                    var bgmVolume = config.Data.BackgroundMusicVolume.Value;
                    var music = audioManager.LoadSound(config.Data.BackgroundMusic, format);
                    music.Source.Volume = bgmVolume;
                    Music = music;
                }
            }

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

        protected override void Dispose(bool disposing) {
            Music?.Dispose();
            Music = null;

            base.Dispose(disposing);
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

        private void PreloadAudio(AudioManager audioManager, AudioControllerConfig.NoteSfxGroup group, List<Sound> sounds) {
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
