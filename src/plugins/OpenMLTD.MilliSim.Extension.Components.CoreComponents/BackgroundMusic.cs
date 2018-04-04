using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
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
        public Sound Music {
            get => _music;
            private set {
                if (_music?.Source != null) {
                    _music.Source.PlaybackStopped -= OnMusicStopped;
                }

                _music = value;

                if (value?.Source != null) {
                    value.Source.PlaybackStopped += OnMusicStopped;
                }
            }
        }

        public void LoadMusic([NotNull] string filePath) {
            var theaterDays = Game.ToBaseGame();

            var audioManager = theaterDays.AudioManager;

            var music = Music;

            if (music != null) {
                audioManager.UnmanageSound(music);

                music.Source.Stop();
                music.Dispose();
            }

            var debug = theaterDays.FindSingleElement<DebugOverlay>();

            if (!File.Exists(filePath)) {
                if (debug != null) {
                    debug.AddLine($"Music file <{filePath}> is not found.");
                }

                Music = null;

                return;
            }

            var config = ConfigurationStore.Get<BackgroundMusicConfig>();
            var volume = config.Data.BackgroundMusicVolume.Value;

            volume = MathHelper.Clamp(volume, 0, 1);

            var format = GetFormatForFile(theaterDays.PluginManager, filePath);

            if (format != null) {
                music = audioManager.LoadSound(filePath, format, false);
                music.Source.Volume = volume;
                Music = music;

                if (debug != null) {
                    debug.AddLine($"Loaded music: {filePath}");
                }
            } else {
                Music = null;

                if (debug != null) {
                    debug.AddLine($"Cannot loaded music <{filePath}> because found no appropriate AudioFormat.");
                }
            }
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            var store = ConfigurationStore;
            var config = store.Get<BackgroundMusicConfig>();

            if (config.Data.BackgroundMusic != null) {
                LoadMusic(config.Data.BackgroundMusic);
            }
        }

        protected override void Dispose(bool disposing) {
            var music = Music;

            if (music != null) {
                if (music.Source != null) {
                    music.Source.PlaybackStopped -= OnMusicStopped;
                }

                music.Dispose();
            }

            Music = null;

            base.Dispose(disposing);
        }

        [CanBeNull]
        private static IAudioFormat GetFormatForFile(BasePluginManager pluginManager, string fileName) {
            return pluginManager.GetPluginsOfType<IAudioFormat>().FirstOrDefault(format => format.SupportsFileType(fileName));
        }

        private void OnMusicStopped(object sender, EventArgs e) {
            var theaterDays = Game.ToBaseGame();
            var syncTimer = theaterDays.FindSingleElement<SyncTimer>();

            if (syncTimer != null) {
                // There will be a duplicate call of Music.Source.Stop(), but it doesn't matter.
                syncTimer.Stop();
            }
        }

        private Sound _music;

    }
}
