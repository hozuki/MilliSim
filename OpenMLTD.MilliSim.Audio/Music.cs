using System;
using System.Diagnostics;
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
            if (IsStopped) {
                OffsetStream.StartTime = currentMixer;
                OffsetStream.CurrentTime = currentMixer;
            } else {
                var newStart = currentMixer - CurrentTime;
                OffsetStream.StartTime = newStart;
                OffsetStream.CurrentTime = currentMixer;
            }

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

            _audioManager.RemoveMusic(this);
            OffsetStream.CurrentTime = OffsetStream.StartTime;

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
                if (value < TimeSpan.Zero) {
                    value = TimeSpan.Zero;
                }

                var currentTime = CurrentTime;

                if (IsStopped && currentTime >= _baseWaveStream.TotalTime && value < _baseWaveStream.TotalTime) {
                    // We used WaveOffsetStream here. This kind of stream will continue playing from start (auto loop)
                    // when stream position reaches the end. This behavior is not wanted. So once the music is stopped,
                    // it cannot go back by setting CurrentTime property. The new playback will start from start.
                    return;
                }

                var shouldStop = false;
                if (value > _baseWaveStream.TotalTime) {
                    value = _baseWaveStream.TotalTime;
                    shouldStop = true;
                }

                var b = value != currentTime;
                if (!b) {
                    return;
                }

                lock (_syncObject) {
                    var waveStream = OffsetStream;

                    if (shouldStop) {
                        Stop();
                        waveStream.CurrentTime = waveStream.StartTime + value;
                        return;
                    }

                    if (!IsPlaying) {
                        var currentMixer = _audioManager.MixerTime;
                        var newStart = currentMixer - currentTime;
                        OffsetStream.StartTime = newStart;
                        OffsetStream.CurrentTime = currentMixer;
                    }

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

        public void UpdateState() {
            if (IsPlaying) {
                ////var t1 = _audioManager.MixerTime;
                ////var t2 = _cachedStartTime;
                //var t1 = OffsetStream.CurrentTime;
                //var t2 = OffsetStream.StartTime;
                //var t3 = OffsetStream.SourceLength;

                //Debug.Print("Abs time: {0}, source time: {1}", t1 - t2, t3);

                //if (t1 - t2 >= t3) {
                //    Stop();
                //}
                if (OffsetStream.CurrentTime - OffsetStream.StartTime >= OffsetStream.SourceLength) {
                    Stop();
                }
            }
        }

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

    }
}
