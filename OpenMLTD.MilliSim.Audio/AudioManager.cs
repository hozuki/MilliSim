using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using NAudio.CoreAudioApi;
using NAudio.Flac;
using NAudio.Vorbis;
using NAudio.Wave;
using NAudio.WindowsMediaFormat;
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

            _soundPlayer.PlaybackStopped += (_, e) => Debug.Print("stopped.");

            _soundPlayer.Play();
        }

        public Music CreateMusic(string fileName, float volume) {
            var audio = LoadAudioFile(fileName);
            return CreateMusic(audio, volume, true);
        }

        public Music CreateMusic(Stream audioData, float volume) {
            var audio = new WaveFileReader(audioData);
            return CreateMusic(audio, volume, true);
        }

        public Music CreateMusic(WaveStream audio, float volume) {
            return CreateMusic(audio, volume, false);
        }

        public Music CreateMusic(WaveStream audio, float volume, bool autoDisposeSource) {
            var music = new Music(this, audio, volume, !autoDisposeSource);
            return music;
        }

        public static bool IsFileSupported(string fileName) {
            var fullName = Path.GetFullPath(fileName);
            var extension = Path.GetExtension(fullName);

            if (extension.StartsWith(".")) {
                extension = extension.TrimStart('.');
            }
            extension = extension.ToLowerInvariant();

            switch (extension) {
                case "wav":
                    return true;
                case "ogg":
                case "oga":
                    // Temporarily disabled. See comments below.
                    return true;
                case "mp3":
                    return true;
                case "flac":
                    return true;
                case "wma":
                    return true;
                default:
                    return false;
            }

            /* Ogg/Vorbis files are currently disabled.
             * MilliSim.Audio uses NAudio.Vorbis, thus NVorbis as its underlying decoding and streaming
             * library. At this point, if you try to seek the WaveStream when the last packets are consumed,
             * you will get a NullReferenceException from PacketReader.FindPacket() (in NVorbis). This
             * even happens when you try to seek to file start (0 sec). The final outcome is, during the
             * following playbacks (second time, third time, etc.), audio will be completely silent.
             * So I have to disable OGG support and hope users can accept only using MP3s.
             *
             * * NVorbis commit: https://github.com/ioctlLR/NVorbis/commit/478eb45c3996e45a6bd19777c0549b85a7bb6851
             * * NAudio.Vorbis commit: https://github.com/naudio/Vorbis/commit/0a16577fa9360304ae2a38d052955eb9e18e012d
             *
             * Temporarily solved by using a custom set of NVorbis and NAudio.Vorbis. Ogg/Vorbis support re-enabled.
             */
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

        internal static WaveStream LoadAudioFile(string fileName) {
            if (!IsFileSupported(fileName)) {
                throw new NotSupportedException();
            }

            var fullName = Path.GetFullPath(fileName);
            var extension = Path.GetExtension(fullName);

            if (extension.StartsWith(".")) {
                extension = extension.TrimStart('.');
            }
            extension = extension.ToLowerInvariant();

            switch (extension) {
                case "wav":
                    return new WaveFileReader(fullName);
                case "ogg":
                case "oga":
                    return new VorbisWaveReader(fullName);
                case "mp3":
                    return new Mp3FileReader(fullName);
                case "flac":
                    return new FlacReader(fullName);
                case "wma":
                    return new WMAFileReader(fullName);
                default:
                    throw new NotSupportedException();
            }
        }

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
