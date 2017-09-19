using System.IO;
using System.Linq;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Audio;
using OpenMLTD.MilliSim.Audio.Extending;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Theater.Configuration.Primitives;
using OpenMLTD.MilliSim.Theater.Elements.Visual.Overlays;
using OpenMLTD.MilliSim.Theater.Extensions;

namespace OpenMLTD.MilliSim.Theater.Elements.Logical {
    public sealed class AudioController : Element {

        public AudioController(GameBase game)
            : base(game) {
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

            var settings = Program.Settings;
            var theaterDays = Game.AsTheaterDays();

            if (settings.Media.BackgroundMusic != null && File.Exists(settings.Media.BackgroundMusic)) {
                var format = GetFormatForFile(Program.PluginManager, settings.Media.BackgroundMusic);
                if (format != null) {
                    var music = theaterDays.AudioManager.CreateMusic(settings.Media.BackgroundMusic, format, settings.Media.BackgroundMusicVolume.Value);
                    theaterDays.AudioManager.AddMusic(music);
                    Music = music;
                }
            }

            var sfx = theaterDays.AudioManager.Sfx;
            PreloadAudio(sfx, settings.Sfx.Tap);
            PreloadAudio(sfx, settings.Sfx.Hold);
            PreloadAudio(sfx, settings.Sfx.Flick);
            PreloadAudio(sfx, settings.Sfx.Slide);
            PreloadAudio(sfx, settings.Sfx.HoldHold);
            PreloadAudio(sfx, settings.Sfx.SlideHold);
            PreloadAudio(sfx, settings.Sfx.HoldEnd);
            PreloadAudio(sfx, settings.Sfx.SlideEnd);
            PreloadAudio(sfx, settings.Sfx.Special);
            PreloadAudio(sfx, settings.Sfx.SpecialHold);
            PreloadAudio(sfx, settings.Sfx.SpecialEnd);

            if (settings.Sfx.Shouts != null) {
                foreach (var shoutPath in settings.Sfx.Shouts) {
                    PreloadAudio(sfx, shoutPath);
                }
            }

            theaterDays.AudioManager.Sfx.Volume = settings.Media.SoundEffectsVolume.Value;
        }

        private void PreloadAudio(SfxManager sfx, string fileName) {
            var debugOverlay = Game.AsTheaterDays().FindSingleElement<DebugOverlay>();
            var pluginManager = Program.PluginManager;
            var format = GetFormatForFile(pluginManager, fileName);
            if (format != null) {
                sfx.PreloadSfx(fileName, format);
            } else {
                if (debugOverlay != null) {
                    debugOverlay.AddLine($"Audio file '{fileName}' is not supported.");
                }
            }
        }

        private void PreloadAudio(SfxManager sfx, NoteSfxGroup group) {
            PreloadAudio(sfx, group.Perfect);
            PreloadAudio(sfx, group.Great);
            PreloadAudio(sfx, group.Nice);
            PreloadAudio(sfx, group.Bad);
            PreloadAudio(sfx, group.Miss);
        }

        [CanBeNull]
        private static IAudioFormat GetFormatForFile(PluginManager pluginManager, string fileName) {
            return pluginManager.AudioFormats.FirstOrDefault(format => format.SupportsFileType(fileName));
        }

        protected override void OnDispose() {
            base.OnDispose();
            Music?.Dispose();
        }

    }
}
