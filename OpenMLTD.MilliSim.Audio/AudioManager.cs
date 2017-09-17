using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using OpenMLTD.MilliSim.Audio.Extending;
using OpenMLTD.MilliSim.Foundation;
using AudioOut = NAudio.Wave.WasapiOut;

namespace OpenMLTD.MilliSim.Audio {
    public class AudioManager : AudioManagerBase {

        public AudioManager() {
            _mixerStream = new WaveMixerStream32 {
                AutoStop = false
            };

            _soundPlayer = new AudioOut(AudioClientShareMode.Shared, 60);
            _soundPlayer.Init(_mixerStream);

            Sfx = new SfxManager(this);

            _soundPlayer.Play();
        }

        public Music CreateMusic([NotNull] string fileName, [NotNull] IAudioFormat format, float volume) {
            var audio = format.Read(fileName);
            return CreateMusic(audio, volume, true);
        }

        private Music CreateMusic(Stream audioData, float volume) {
            var audio = new WaveFileReader(audioData);
            return CreateMusic(audio, volume, true);
        }

        private Music CreateMusic(WaveStream audio, float volume) {
            return CreateMusic(audio, volume, false);
        }

        public Music CreateMusic(WaveStream audio, float volume, bool autoDisposeSource) {
            var music = new Music(this, audio, volume, !autoDisposeSource);
            return music;
        }

        public void AddMusic([NotNull] Music music) {
            if (music.IsPlaying) {
                AddInputStream(music.OffsetStream, music.CachedVolume);
            }
        }

        public void RemoveMusic([NotNull] Music music) {
            RemoveInputStream(music.OffsetStream);
        }

        public SfxManager Sfx { get; }

        internal TimeSpan MixerTime => _mixerStream.CurrentTime;

        internal WaveChannel32 AddInputStream([NotNull] WaveStream waveStream, float volume) {
            lock (_channelLock) {
                if (_channels.ContainsKey(waveStream)) {
                    return _channels[waveStream];
                }

                var channel = new WaveChannel32(waveStream, volume, 0f);
                _channels.Add(waveStream, channel);

                _mixerStream.AddInputStream(channel);

                return channel;
            }
        }

        internal void RemoveInputStream([NotNull] WaveStream waveStream) {
            lock (_channelLock) {
                if (!_channels.ContainsKey(waveStream)) {
                    return;
                }

                var ch = _channels[waveStream];

                _mixerStream.RemoveInputStream(ch);

                _channels.Remove(waveStream);
            }
        }

        internal float GetStreamVolume([NotNull] WaveStream waveStream) {
            lock (_channelLock) {
                return _channels[waveStream].Volume;
            }
        }

        internal void SetStreamVolume([NotNull] WaveStream waveStream, float volume) {
            lock (_channelLock) {
                if (!_channels.ContainsKey(waveStream)) {
                    return;
                }
                _channels[waveStream].Volume = volume;
            }
        }

        internal TimeSpan GetStreamCurrentTime([NotNull] WaveStream waveStream) {
            lock (_channelLock) {
                return _channels[waveStream].CurrentTime;
            }
        }

        internal void SetStreamCurrentTime([NotNull] WaveStream waveStream, TimeSpan time) {
            lock (_channelLock) {
                if (!_channels.ContainsKey(waveStream)) {
                    return;
                }
                _channels[waveStream].CurrentTime = time;
            }
        }

        internal WaveChannel32 GetChannelOf([NotNull] WaveStream waveStream) {
            lock (_channelLock) {
                return _channels[waveStream];
            }
        }

        private WaveFormat RequiredFormat => _mixerStream.WaveFormat;

        protected override void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }

            _soundPlayer.Stop();

            Sfx.StopAll();
            Sfx.Dispose();

            _mixerStream.Dispose();
            _soundPlayer.Dispose();
        }

        private readonly AudioOut _soundPlayer;
        private readonly object _channelLock = new object();

        private readonly WaveMixerStream32 _mixerStream;

        private readonly Dictionary<WaveStream, WaveChannel32> _channels = new Dictionary<WaveStream, WaveChannel32>();

    }
}
