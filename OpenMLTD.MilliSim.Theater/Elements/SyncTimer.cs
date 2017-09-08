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

        /// <summary>
        /// Gets/sets current time synchorization target.
        /// </summary>
        public TimerSyncTarget SyncTarget { get; set; }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            var theaterDays = Game.AsTheaterDays();

            var audio = theaterDays.FindSingleElement<AudioController>();
            var video = theaterDays.FindSingleElement<BackgroundVideo>();

            if (SyncTarget == TimerSyncTarget.Auto) {
                var timeFilled = false;

                if (!timeFilled) {
                    if (audio?.Music != null) {
                        CurrentTime = audio.Music.CurrentTime;
                        timeFilled = true;
                    }
                }

                if (!timeFilled) {
                    if (video != null) {
                        CurrentTime = video.CurrentTime;
                        timeFilled = true;
                    }
                }

                if (!timeFilled) {
                    CurrentTime = gameTime.Total;
                }
            } else {
                switch (SyncTarget) {
                    case TimerSyncTarget.Audio:
                        if (audio?.Music != null) {
                            CurrentTime = audio.Music.CurrentTime;
                        }
                        break;
                    case TimerSyncTarget.Video:
                        if (video != null) {
                            CurrentTime = video.CurrentTime;
                        }
                        break;
                    case TimerSyncTarget.GameTime:
                        CurrentTime = gameTime.Total;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

    }
}
