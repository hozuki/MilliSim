using System.IO;
using System.Linq;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Audio;
using OpenMLTD.MilliSim.Audio.Extending;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.GameAbstraction;
using OpenMLTD.MilliSim.GameAbstraction.Extensions;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public sealed class AudioController : Component {

        public AudioController([NotNull] IComponentContainer parent)
            : base(parent) {
        }

        [CanBeNull]
        public Music Music { get; private set; }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            var theaterDays = Game.AsTheaterDays();
            theaterDays.AudioManager.Sfx.UpdateWaveQueue();

            Music?.UpdateState();
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            var theaterDays = Game.AsTheaterDays();
            var store = ConfigurationStore;
            var config = store.Get<AudioControllerConfig>();

            if (config.Data.BackgroundMusic != null && File.Exists(config.Data.BackgroundMusic)) {
                var format = GetFormatForFile(theaterDays.PluginManager, config.Data.BackgroundMusic);
                if (format != null) {
                    var bgmVolume = config.Data.BackgroundMusicVolume.Value;
                    var music = theaterDays.AudioManager.CreateMusic(config.Data.BackgroundMusic, format, bgmVolume);
                    theaterDays.AudioManager.AddMusic(music);
                    Music = music;
                }
            }

            var sfx = theaterDays.AudioManager.Sfx;
            PreloadAudio(sfx, config.Data.Sfx.Tap);
            PreloadAudio(sfx, config.Data.Sfx.Hold);
            PreloadAudio(sfx, config.Data.Sfx.Flick);
            PreloadAudio(sfx, config.Data.Sfx.Slide);
            PreloadAudio(sfx, config.Data.Sfx.HoldHold);
            PreloadAudio(sfx, config.Data.Sfx.SlideHold);
            PreloadAudio(sfx, config.Data.Sfx.HoldEnd);
            PreloadAudio(sfx, config.Data.Sfx.SlideEnd);
            PreloadAudio(sfx, config.Data.Sfx.Special);
            PreloadAudio(sfx, config.Data.Sfx.SpecialHold);
            PreloadAudio(sfx, config.Data.Sfx.SpecialEnd);

            if (config.Data.Sfx.Shouts != null) {
                foreach (var shoutPath in config.Data.Sfx.Shouts) {
                    PreloadAudio(sfx, shoutPath);
                }
            }

            theaterDays.AudioManager.Sfx.Volume = config.Data.SfxVolume.Value;
        }

        private void PreloadAudio(SfxManager sfx, string fileName) {
            var theaterDays = Game.AsTheaterDays();
            var debugOverlay = theaterDays.FindSingleElement<DebugOverlay>();
            var pluginManager = theaterDays.PluginManager;
            var format = GetFormatForFile(pluginManager, fileName);
            if (format != null) {
                sfx.PreloadSfx(fileName, format);
            } else {
                if (debugOverlay != null) {
                    debugOverlay.AddLine($"Audio file '{fileName}' is not supported.");
                }
            }
        }

        private void PreloadAudio(SfxManager sfx, AudioControllerConfig.NoteSfxGroup group) {
            PreloadAudio(sfx, group.Perfect);
            PreloadAudio(sfx, group.Great);
            PreloadAudio(sfx, group.Nice);
            PreloadAudio(sfx, group.Bad);
            PreloadAudio(sfx, group.Miss);
        }

        [CanBeNull]
        private static IAudioFormat GetFormatForFile(PluginManager pluginManager, string fileName) {
            return pluginManager.GetPluginsOfType<IAudioFormat>().FirstOrDefault(format => format.SupportsFileType(fileName));
        }

        protected override void OnDispose() {
            base.OnDispose();
            Music?.Dispose();
        }

    }
}
