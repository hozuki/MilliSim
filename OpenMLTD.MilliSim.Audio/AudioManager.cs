using System;
using System.Collections.Generic;
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
            _mixerStream = new WaveMixerStream32();
            _soundPlayer = new AudioOut(AudioClientShareMode.Shared, 60);
            _soundPlayer.Init(_mixerStream);
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
                case "ogg":
                case "oga":
                case "mp3":
                case "flac":
                case "wma":
                    return true;
                default:
                    return false;
            }
        }

        public Music Music {
            get => _music;
            set {
                if (_music != null) {
                    _music.Stop();
                    _mixerStream.RemoveInputStream(_music.Channel);
                }
                if (value != null) {
                    _mixerStream.AddInputStream(value.Channel);
                }
                _music = value;
            }
        }

        internal void ForceMixerOutput() {
            _soundPlayer.Play();
        }

        internal bool NeedSampleRateConversionFrom(WaveFormat sourceFormat) {
            if (_mixerStream.InputCount == 0) {
                return false;
            }
            var standard = StandardFormat;
            return sourceFormat.SampleRate != standard.SampleRate ||
                   sourceFormat.BitsPerSample != standard.BitsPerSample ||
                   sourceFormat.Channels != standard.Channels ||
                   sourceFormat.Encoding != standard.Encoding;
        }

        internal WaveFormat StandardFormat => _mixerStream.WaveFormat;

        internal AudioOut AudioOut => _soundPlayer;

        protected override void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }

            _soundPlayer.Stop();

            _mixerStream.Dispose();
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
        private readonly WaveMixerStream32 _mixerStream;

    }
}
