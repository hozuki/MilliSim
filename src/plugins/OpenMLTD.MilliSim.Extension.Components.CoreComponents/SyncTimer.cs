using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using SharpAL;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    /// <inheritdoc />
    /// <summary>
    /// The synchronization timer.
    /// </summary>
    public sealed class SyncTimer : BaseGameComponent {

        /// <summary>
        /// Creates a new <see cref="SyncTimer"/> instance.
        /// </summary>
        /// <param name="game">The base game.</param>
        /// <param name="parent">The parent of the <see cref="SyncTimer"/>.</param>
        public SyncTimer([NotNull] BaseGame game, [NotNull] IBaseGameComponentContainer parent)
            : base(game, parent) {
        }

        /// <summary>
        /// Triggered when the <see cref="SyncTimer"/>'s state is changed.
        /// </summary>
        public event EventHandler<SyncTimerStateChangedEventArgs> StateChanged;

        /// <summary>
        /// Gets current time.
        /// </summary>
        public TimeSpan CurrentTime { get; private set; } = TimeSpan.Zero;

        /// <summary>
        /// Gets or sets current time synchronization target.
        /// </summary>
        public TimerSyncTarget SyncTarget { get; set; }

        /// <summary>
        /// Gets or sets current time synchronization method.
        /// </summary>
        public TimerSyncMethod SyncMethod { get; set; }

        /// <summary>
        /// Starts timing.
        /// </summary>
        public void Start() {
            if (_backgroundVideo != null) {
                if (_backgroundVideo.State == MediaState.Stopped) {
                    _backgroundVideo.Play();
                } else {
                    _backgroundVideo.Resume();
                }
            }

            var source = GetMusicSource();
            source?.PlayDirect();
            _stopwatch.Start();

            StateChanged?.Invoke(this, new SyncTimerStateChangedEventArgs(MediaState.Playing));
        }

        /// <summary>
        /// Pauses timing.
        /// </summary>
        public void Pause() {
            _backgroundVideo?.Pause();
            var source = GetMusicSource();
            source?.Pause();
            _stopwatch.Stop();

            StateChanged?.Invoke(this, new SyncTimerStateChangedEventArgs(MediaState.Paused));
        }

        /// <summary>
        /// Stops timing.
        /// </summary>
        public void Stop() {
            _backgroundVideo?.Stop();
            var source = GetMusicSource();
            source?.Stop();
            _stopwatch.Reset();

            StateChanged?.Invoke(this, new SyncTimerStateChangedEventArgs(MediaState.Stopped));
        }

        /// <summary>
        /// Seeks to a specified time.
        /// </summary>
        /// <param name="targetTime">The time to seek to.</param>
        public void Seek(TimeSpan targetTime) {
            if (_backgroundVideo != null) {
                // TODO: Implement seeking function for BackgroundVideo
                //_backgroundVideo.CurrentTime = targetTime;
            }

            var source = GetMusicSource();

            if (source != null) {
                source.CurrentTime = targetTime;
            }

            _soughtTime = targetTime;
            _stopwatch.Restart();
        }

        /// <summary>
        /// Gets a <see cref="bool"/> indicating if the timer is running.
        /// </summary>
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

            var audio = theaterDays.FindFirstElementOrDefault<BackgroundMusic>();
            var video = theaterDays.FindFirstElementOrDefault<BackgroundVideo>();

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CanBeNull]
        private AudioSource GetMusicSource() {
            return _backgroundMusic?.Music?.Source;
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
