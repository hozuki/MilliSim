using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public sealed class SyncTimer : BaseGameComponent {

        public SyncTimer([NotNull] BaseGame game, [NotNull] IBaseGameComponentContainer parent)
            : base(game, parent) {
        }

        public TimeSpan CurrentTime { get; private set; } = TimeSpan.Zero;

        /// <summary>
        /// Gets/sets current time synchorization target.
        /// </summary>
        public TimerSyncTarget SyncTarget { get; set; }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            var theaterDays = Game.ToBaseGame();

            var audio = theaterDays.FindSingleElement<AudioController>();
            var video = theaterDays.FindSingleElement<BackgroundVideo>();

            if (SyncTarget == TimerSyncTarget.Auto) {
                var timeFilled = false;

                if (audio?.Music != null) {
                    CurrentTime = audio.Music.Source.CurrentTime;
                    timeFilled = true;
                }

                if (!timeFilled) {
                    if (video != null) {
                        CurrentTime = video.CurrentTime;
                        timeFilled = true;
                    }
                }

                if (!timeFilled) {
                    CurrentTime = gameTime.TotalGameTime;
                }
            } else {
                switch (SyncTarget) {
                    case TimerSyncTarget.Audio:
                        if (audio?.Music != null) {
                            CurrentTime = audio.Music.Source.CurrentTime;
                        }
                        break;
                    case TimerSyncTarget.Video:
                        if (video != null) {
                            CurrentTime = video.CurrentTime;
                        }
                        break;
                    case TimerSyncTarget.GameTime:
                        CurrentTime = gameTime.TotalGameTime;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

    }
}
