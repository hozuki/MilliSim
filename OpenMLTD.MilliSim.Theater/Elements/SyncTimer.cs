using System;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Theater.Extensions;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public sealed class SyncTimer : Element {

        public SyncTimer(GameBase game)
            : base(game) {
        }

        public TimeSpan CurrentTime { get; private set; } = TimeSpan.Zero;

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            var theaterDays = Game.AsTheaterDays();
            var timeFilled = false;

            if (!timeFilled) {
                var audio = theaterDays.FindSingleElement<AudioController>();
                if (audio?.Music != null) {
                    CurrentTime = audio.Music.CurrentTime;
                    timeFilled = true;
                }
            }

            if (!timeFilled) {
                var video = theaterDays.FindSingleElement<BackgroundVideo>();
                if (video != null) {
                    CurrentTime = video.CurrentTime;
                    timeFilled = true;
                }
            }

            if (!timeFilled) {
                CurrentTime = gameTime.Total;
            }
        }

    }
}
