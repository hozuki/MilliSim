using System;
using System.IO;
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
            MixerStream = new WaveMixerStream32 {
                AutoStop = true
            };
            _soundPlayer = new AudioOut(AudioClientShareMode.Shared, 60);
            _soundPlayer.Init(MixerStream);
        }

        public Music CreateMusic(string fileName) {
            var audio = LoadAudioFile(fileName);
            return CreateMusic(audio, true);
        }

        public Music CreateMusic(Stream audioData) {
            var audio = new WaveFileReader(audioData);
            return CreateMusic(audio, true);
        }

        public Music CreateMusic(WaveStream audio) {
            return CreateMusic(audio, false);
        }

        public Music CreateMusic(WaveStream audio, bool autoDisposeSource) {
            var music = new Music(this, audio, !autoDisposeSource);
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

        public Music Music {
            get => _music;
            set {
                if (_music != null) {
                    _music.Stop();
                    MixerStream.RemoveInputStream(_music.Channel);
                }
                if (value != null) {
                    MixerStream.CurrentTime = TimeSpan.Zero;
                    MixerStream.AddInputStream(value.Channel);
                }
                _music = value;
            }
        }

        internal WaveMixerStream32 MixerStream { get; }

        internal bool NeedSampleRateConversionFrom(WaveFormat sourceFormat) {
            if (MixerStream.InputCount == 0) {
                return false;
            }
            var standard = StandardFormat;
            return sourceFormat.SampleRate != standard.SampleRate ||
                   sourceFormat.BitsPerSample != standard.BitsPerSample ||
                   sourceFormat.Channels != standard.Channels ||
                   sourceFormat.Encoding != standard.Encoding;
        }

        internal WaveFormat StandardFormat => MixerStream.WaveFormat;

        internal AudioOut AudioOut => _soundPlayer;

        protected override void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }

            _soundPlayer.Stop();

            MixerStream.Dispose();
            _soundPlayer.Dispose();
        }

        private static WaveStream LoadAudioFile(string fileName) {
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

        private Music _music;
        private readonly AudioOut _soundPlayer;

    }
}
