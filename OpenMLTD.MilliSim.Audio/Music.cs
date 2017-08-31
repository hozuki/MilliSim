using System;
using JetBrains.Annotations;
using NAudio.Wave;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Audio {
    public sealed class Music : DisposableBase {

        internal Music(AudioManager audioManager, [NotNull] WaveStream waveStream, bool externalWaveStream) {
            AudioManager = audioManager;
            BaseWaveStream = waveStream;
            _isExternalWaveStream = externalWaveStream;

            if (audioManager.NeedSampleRateConversionFrom(waveStream.WaveFormat)) {
                FormatConvertedWaveStream = new ResamplerDmoStream(waveStream, audioManager.StandardFormat);
            } else {
                FormatConvertedWaveStream = waveStream;
            }

            Channel = new WaveChannel32(FormatConvertedWaveStream, 1f, 0f);
        }

        public event EventHandler<EventArgs> CurrentTimeRepositioned;

        public void Play() {
            if (IsPlaying) {
                return;
            }

            AudioManager.AudioOut.Play();
            AudioManager.ForceMixerOutput();

            IsPaused = false;
            IsStopped = false;
            IsPlaying = true;
        }

        public void Pause() {
            if (IsStopped || IsPaused) {
                return;
            }

            AudioManager.AudioOut.Pause();

            IsPlaying = false;
            IsPaused = true;
        }

        public void Stop() {
            if (IsStopped) {
                return;
            }

            AudioManager.AudioOut.Stop();

            IsPlaying = false;
            IsPaused = false;
            IsStopped = true;
        }

        public float Volume {
            get => Channel.Volume;
            set => Channel.Volume = value.Clamp(0, 1);
        }

        public float Pan {
            get => Channel.Pan;
            set => Channel.Pan = value;
        }

        public bool IsPlaying { get; private set; }

        public bool IsPaused { get; private set; }

        public bool IsStopped { get; private set; }

        public TimeSpan CurrentTime {
            get => Channel.CurrentTime;
            set {
                var b = value != CurrentTime;
                if (!b) {
                    return;
                }

                lock (_syncObject) {
                    var waveStream = Channel;
                    waveStream.CurrentTime = value;

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

                CurrentTimeRepositioned?.Invoke(this, EventArgs.Empty);
            }
        }

        public TimeSpan TotalTime => BaseWaveStream.TotalTime;

        internal AudioManager AudioManager { get; }

        internal WaveStream BaseWaveStream { get; }

        internal WaveStream FormatConvertedWaveStream { get; }

        internal WaveChannel32 Channel { get; }

        protected override void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }

            Stop();

            Channel.Dispose();
            if (FormatConvertedWaveStream != BaseWaveStream) {
                FormatConvertedWaveStream.Dispose();
            }
            if (!_isExternalWaveStream) {
                BaseWaveStream.Dispose();
            }
        }

        private readonly bool _isExternalWaveStream;
        private readonly object _syncObject = new object();

    }
}
