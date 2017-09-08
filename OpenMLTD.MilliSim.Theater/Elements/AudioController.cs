using System;
using System.IO;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Audio;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public sealed class AudioController : Element {

        public AudioController(GameBase game)
            : base(game) {
        }

        [CanBeNull]
        public Music Music { get; private set; }

        public TimeSpan CorrectedCurrentTime { get; private set; } = TimeSpan.Zero;

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);
            if (Music != null) {
                CorrectedCurrentTime = Music.CurrentTime;
            }
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            var settings = Program.Settings;
            var theaterDays = GetTypedGame();
            if (settings.Media.BackgroundMusic != null && File.Exists(settings.Media.BackgroundMusic) &&
                AudioManager.IsFileSupported(settings.Media.BackgroundMusic)) {
                var music = theaterDays.AudioManager.CreateMusic(settings.Media.BackgroundMusic);
                theaterDays.AudioManager.Music = music;
                music.Volume = settings.Media.BackgroundMusicVolume.Value;
                Music = music;
            }
        }

        protected override void OnDispose() {
            base.OnDispose();
            Music?.Dispose();
        }

        private TheaterDays GetTypedGame() => (TheaterDays)Game;

    }
}
