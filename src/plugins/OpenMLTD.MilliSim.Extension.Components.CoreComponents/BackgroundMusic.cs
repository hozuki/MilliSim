using System.IO;
using System.Linq;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Audio;
using OpenMLTD.MilliSim.Audio.Extending;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public sealed class BackgroundMusic : BaseGameComponent {

        public BackgroundMusic([NotNull] BaseGame game, [NotNull] IBaseGameComponentContainer parent)
            : base(game, parent) {
        }

        [CanBeNull]
        public Sound Music { get; private set; }

        protected override void OnInitialize() {
            base.OnInitialize();

            var theaterDays = Game.ToBaseGame();
            var store = ConfigurationStore;
            var config = store.Get<BackgroundMusicConfig>();

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
        }

        protected override void Dispose(bool disposing) {
            Music?.Dispose();
            Music = null;

            base.Dispose(disposing);
        }

        [CanBeNull]
        private static IAudioFormat GetFormatForFile(BasePluginManager pluginManager, string fileName) {
            return pluginManager.GetPluginsOfType<IAudioFormat>().FirstOrDefault(format => format.SupportsFileType(fileName));
        }

    }
}
