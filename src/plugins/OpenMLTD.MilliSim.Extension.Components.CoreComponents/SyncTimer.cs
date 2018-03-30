using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public sealed class SyncTimer : BaseGameComponent {

        public SyncTimer([NotNull] BaseGame game, [NotNull] IBaseGameComponentContainer parent)
            : base(game, parent) {
        }

        public event EventHandler<SyncTimerStateChangedEventArgs> StateChanged;

        public TimeSpan CurrentTime { get; private set; } = TimeSpan.Zero;

        /// <summary>
        /// Gets/sets current time synchorization target.
        /// </summary>
        public TimerSyncTarget SyncTarget { get; set; }

        public TimerSyncMethod SyncMethod { get; set; }

        public void Start() {
            if (_backgroundVideo != null) {
                if (_backgroundVideo.State == MediaState.Stopped) {
                    _backgroundVideo.Play();
                } else {
                    _backgroundVideo.Resume();
                }
            }

            _backgroundMusic?.Music?.Source.PlayDirect();
            _stopwatch.Start();

            StateChanged?.Invoke(this, new SyncTimerStateChangedEventArgs(MediaState.Playing));
        }

        public void Pause() {
            _backgroundVideo?.Pause();
            _backgroundMusic?.Music?.Source.Pause();
            _stopwatch.Stop();

            StateChanged?.Invoke(this, new SyncTimerStateChangedEventArgs(MediaState.Paused));
        }

        public void Stop() {
            _backgroundVideo?.Stop();
            _backgroundMusic?.Music?.Source.Stop();
            _stopwatch.Reset();

            StateChanged?.Invoke(this, new SyncTimerStateChangedEventArgs(MediaState.Stopped));
        }

        public void Seek(TimeSpan targetTime) {
            if (_backgroundVideo != null) {
                // TODO: Implement seeking function for BackgroundVideo
                //_backgroundVideo.CurrentTime = targetTime;
            }

            if (_backgroundMusic?.Music != null) {
                _backgroundMusic.Music.Source.CurrentTime = targetTime;
            }

            _soughtTime = targetTime;
            _stopwatch.Restart();
        }

        public bool IsRunning => _stopwatch.IsRunning;

        internal readonly object UpdateLock = new object();

        internal void RaiseStateChanged([NotNull] object sender, [NotNull] SyncTimerStateChangedEventArgs e) {
            StateChanged?.Invoke(sender, e);
        }

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
                    var audioSource = audio.Music.Source;

                    if (audioSource == null) {
                        standardTime = TimeSpan.Zero;
                    } else {
                        standardTime = audioSource.CurrentTime;
                    }

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
                    standardTime = _stopwatch.Elapsed + _soughtTime;
                }
            } else {
                switch (SyncTarget) {
                    case TimerSyncTarget.Audio:
                        if (audio?.Music != null) {
                            var audioSource = audio.Music.Source;

                            if (audioSource == null) {
                                standardTime = TimeSpan.Zero;
                            } else {
                                standardTime = audioSource.CurrentTime;
                            }

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
                    case TimerSyncTarget.Stopwach:
                        standardTime = _stopwatch.Elapsed + _soughtTime;
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
        private TimeSpan _soughtTime = TimeSpan.Zero;

        private static readonly TimeSpan MaxEstimationError = TimeSpan.FromSeconds(0.1);

    }
}
