using System;
using JetBrains.Annotations;
using NAudio.Wave;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Audio {
    public sealed class Music : DisposableBase {

        internal Music(AudioManager audioManager, [NotNull] WaveStream waveStream, float volume, bool externalWaveStream) {
            _audioManager = audioManager;
            _baseWaveStream = waveStream;
            _isExternalWaveStream = externalWaveStream;

            if (AudioHelper.NeedsFormatConversionFrom(waveStream.WaveFormat, RequiredFormat)) {
                _formatConvertedStream = new ResamplerDmoStream(waveStream, RequiredFormat);
            } else {
                _formatConvertedStream = waveStream;
            }

            OffsetStream = new WaveOffsetStream(_formatConvertedStream);

            IsStopped = true;

            CachedVolume = volume;
        }

        public void Play() {
            if (IsPlaying) {
                return;
            }

            var currentMixer = _audioManager.MixerTime;
            var lastMixer = _lastPausedMixerTime;
            var relStart = lastMixer - OffsetStream.StartTime;
            var pausedTime = currentMixer - lastMixer;
            var newStart = relStart + pausedTime;
            OffsetStream.StartTime = newStart;
            OffsetStream.CurrentTime = currentMixer;
            _audioManager.AddInputStream(OffsetStream, CachedVolume);
            
            IsPaused = false;
            IsStopped = false;
            IsPlaying = true;
        }

        public void Pause() {
            if (IsStopped || IsPaused) {
                return;
            }

            if (IsPlaying) {
                CachedVolume = Volume;
            }

            _lastPausedMixerTime = _audioManager.MixerTime;
            _audioManager.RemoveMusic(this);

            IsPlaying = false;
            IsPaused = true;
        }

        public void Stop() {
            if (IsStopped) {
                return;
            }

            if (IsPlaying) {
                CachedVolume = Volume;
            }

            _lastPausedMixerTime = TimeSpan.Zero;
            _audioManager.RemoveMusic(this);
            OffsetStream.CurrentTime = TimeSpan.Zero;

            IsPlaying = false;
            IsPaused = false;
            IsStopped = true;
        }

        public bool IsPlaying { get; private set; }

        public bool IsPaused { get; private set; }

        public bool IsStopped { get; private set; }

        public TimeSpan CurrentTime {
            get => OffsetStream.CurrentTime - OffsetStream.StartTime;
            set {
                var b = value != CurrentTime;
                if (!b) {
                    return;
                }

                lock (_syncObject) {
                    var waveStream = OffsetStream;
                    waveStream.CurrentTime = waveStream.StartTime + value;

                    var position = waveStream.Position;
                    var blockAlign = waveStream.BlockAlign;
                    if (position % blockAlign != 0) {
                        position = (long)(Math.Round(position / (double)blockAlign) * blockAlign);
                        if (position < 0) {
                            position = 0;
                        }
                        waveStream.Position = position;
                    }
                }
            }
        }

        public TimeSpan TotalTime => _baseWaveStream.TotalTime;

        internal WaveOffsetStream OffsetStream { get; }

        internal float CachedVolume { get; private set; }

        protected override void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }

            Stop();

            if (_formatConvertedStream != _baseWaveStream) {
                _formatConvertedStream.Dispose();
            }

            OffsetStream.Dispose();
            if (!_isExternalWaveStream) {
                _baseWaveStream.Dispose();
            }
        }

        private static WaveFormat RequiredFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);

        private float Volume {
            get => _audioManager.GetStreamVolume(OffsetStream);
            set => _audioManager.SetStreamVolume(OffsetStream, value);
        }

        private readonly AudioManager _audioManager;
        private readonly WaveStream _baseWaveStream;
        private readonly WaveStream _formatConvertedStream;
        private readonly bool _isExternalWaveStream;
        private readonly object _syncObject = new object();
        private TimeSpan _lastPausedMixerTime = TimeSpan.Zero;

    }
}
