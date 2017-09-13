using System;
using System.IO;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Audio;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Theater.Configuration.Primitives;
using OpenMLTD.MilliSim.Theater.Extensions;

namespace OpenMLTD.MilliSim.Theater.Elements {
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

            if (settings.Media.BackgroundMusic != null && File.Exists(settings.Media.BackgroundMusic) &&
                AudioManager.IsFileSupported(settings.Media.BackgroundMusic)) {
                var music = theaterDays.AudioManager.CreateMusic(settings.Media.BackgroundMusic, settings.Media.BackgroundMusicVolume.Value);
                theaterDays.AudioManager.AddMusic(music);
                Music = music;
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

            theaterDays.AudioManager.Sfx.Volume = settings.Media.SoundEffectsVolume.Value;
        }

        private static void PreloadAudio(SfxManager sfx, string fileName) {
            sfx.PreloadSfx(fileName);
        }

        private static void PreloadAudio(SfxManager sfx, NoteSfxGroup @group) {
            sfx.PreloadSfx(group.Perfect);
            sfx.PreloadSfx(group.Great);
            sfx.PreloadSfx(group.Nice);
            sfx.PreloadSfx(group.Bad);
            sfx.PreloadSfx(group.Miss);
        }

        protected override void OnDispose() {
            base.OnDispose();
            Music?.Dispose();
        }

    }
}
