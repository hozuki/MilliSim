using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration;
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

        public TimerSyncMethod SyncMethod { get; set; }

        [NotNull]
        public Stopwatch Stopwatch => _stopwatch;

        protected override void OnInitialize() {
            base.OnInitialize();

            var config = ConfigurationStore.Get<SyncTimerConfig>();

            SyncTarget = config.Data.SyncTarget;
            SyncMethod = config.Data.SyncMethod;

            var theaterDays = Game.ToBaseGame();

            var audio = theaterDays.FindSingleElement<BackgroundMusic>();
            var video = theaterDays.FindSingleElement<BackgroundVideo>();

            _backgroundMusic = audio;
            _backgroundVideo = video;
        }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            var audio = _backgroundMusic;
            var video = _backgroundVideo;

            var canCompensate = false;
            var standardTime = TimeSpan.Zero;

            if (SyncTarget == TimerSyncTarget.Auto) {
                var timeFilled = false;

                if (audio?.Music != null) {
                    standardTime = audio.Music.Source.CurrentTime;
                    timeFilled = true;
                    canCompensate = true;
                }

                if (!timeFilled) {
                    if (video != null) {
                        standardTime = video.CurrentTime;
                        timeFilled = true;
                        canCompensate = true;
                    }
                }

                if (!timeFilled) {
                    standardTime = gameTime.TotalGameTime;
                }
            } else {
                switch (SyncTarget) {
                    case TimerSyncTarget.Audio:
                        if (audio?.Music != null) {
                            standardTime = audio.Music.Source.CurrentTime;
                            canCompensate = true;
                        }
                        break;
                    case TimerSyncTarget.Video:
                        if (video != null) {
                            standardTime = video.CurrentTime;
                            canCompensate = true;
                        }
                        break;
                    case TimerSyncTarget.GameTime:
                        standardTime = gameTime.TotalGameTime;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (!canCompensate || SyncMethod == TimerSyncMethod.Naive) {
                CurrentTime = standardTime;
            } else {
                var stopwatchTime = _stopwatch.Elapsed;
                var estimatedTime = stopwatchTime - _lastStopwatchDifference;
                var newDifference = estimatedTime - standardTime;

                if (newDifference > MaxEstimationError || newDifference < -MaxEstimationError) {
                    _lastStopwatchDifference += newDifference;
                    CurrentTime = standardTime;
                } else {
                    CurrentTime = estimatedTime;
                }
            }
        }

        [CanBeNull]
        private BackgroundVideo _backgroundVideo;
        [CanBeNull]
        private BackgroundMusic _backgroundMusic;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        private TimeSpan _lastStopwatchDifference;

        private static readonly TimeSpan MaxEstimationError = TimeSpan.FromSeconds(0.1);

    }
}
