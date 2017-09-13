using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using NAudio.Wave;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Audio {
    public sealed class SfxManager : DisposableBase {

        internal SfxManager(AudioManager audioManager) {
            _audioManager = audioManager;
        }

        public void PreloadSfx([CanBeNull] string fileName) {
            if (fileName == null) {
                return;
            }

            fileName = Path.GetFullPath(fileName);

            if (!File.Exists(fileName)) {
                throw new FileNotFoundException("Audio file is not found.", fileName);
            }
            if (!AudioManager.IsFileSupported(fileName)) {
                throw new FormatException($"File '{fileName}' is not supported.");
            }

            var key = Environment.OSVersion.Platform == PlatformID.Win32NT ? fileName.ToLowerInvariant() : fileName;

            if (_preloaded.ContainsKey(key)) {
                return;
            }

            using (var waveStream = AudioManager.LoadAudioFile(fileName)) {
                byte[] data;
                var buf = new byte[1024];

                using (var memoryStream = new MemoryStream()) {
                    var read = waveStream.Read(buf, 0, buf.Length);
                    while (read > 0) {
                        memoryStream.Write(buf, 0, read);

                        if (read < buf.Length) {
                            break;
                        }

                        read = waveStream.Read(buf, 0, buf.Length);
                    }
                    data = memoryStream.ToArray();
                }

                var format = waveStream.WaveFormat;
                _preloaded.Add(key, (data, format));
            }
        }

        public void DiscardAllPreloadedSfx() {
            _preloaded.Clear();
        }

        public float Volume { get; set; } = 1f;

        public void Play([CanBeNull] string fileName) {
            if (fileName == null) {
                return;
            }

            fileName = Path.GetFullPath(fileName);

            PreloadSfx(fileName);

            var key = Environment.OSVersion.Platform == PlatformID.Win32NT ? fileName.ToLowerInvariant() : fileName;

            var currentTime = _audioManager.MixerTime;

            var free = GetFreeStream(key);
            if (free.OffsetStream != null) {
                free.OffsetStream.StartTime = currentTime;
                free.OffsetStream.CurrentTime = currentTime;
                _playingStates[free.Index] = true;
                _audioManager.AddInputStream(free.OffsetStream, Volume);
                return;
            }

            var (data, format) = _preloaded[key];

            var source = new RawSourceWaveStream(data, 0, data.Length, format);

            // Offset requires 16-bit integer input.
            WaveStream toOffset;
            if (AudioHelper.NeedsFormatConversionFrom(format, RequiredFormat)) {
                toOffset = new ResamplerDmoStream(source, RequiredFormat);
            } else {
                toOffset = source;
            }

            var offset = new WaveOffsetStream(toOffset, currentTime, TimeSpan.Zero, toOffset.TotalTime);

            _audioManager.AddInputStream(offset, Volume);

            lock (_queueLock) {
                _playingWaveStreams.Add((key, offset, toOffset, source));
            }
            _playingStates.Add(true);
        }

        public void StopAll() {
            lock (_queueLock) {
                foreach (var (_, offset, toOffset, source) in _playingWaveStreams) {
                    _audioManager.RemoveInputStream(offset);
                    offset.Dispose();
                    toOffset.Dispose();
                    if (toOffset != source) {
                        source.Dispose();
                    }
                }
                _playingWaveStreams.Clear();
                _playingStates.Clear();
            }
        }

        public void UpdateWaveQueue() {
            lock (_queueLock) {
                for (var i = 0; i < _playingWaveStreams.Count; ++i) {
                    if (!_playingStates[i]) {
                        continue;
                    }

                    var (_, stream, _, _) = _playingWaveStreams[i];
                    if (stream.Position >= stream.Length) {
                        _playingStates[i] = false;
                        _audioManager.RemoveInputStream(stream);
                    }
                }
            }
        }

        private (WaveOffsetStream OffsetStream, int Index) GetFreeStream(string key) {
            if (!_preloaded.ContainsKey(key)) {
                return (null, -1);
            }

            lock (_queueLock) {
                for (var i = 0; i < _playingWaveStreams.Count; ++i) {
                    if (_playingStates[i]) {
                        continue;
                    }

                    var (k, offset, _, _) = _playingWaveStreams[i];
                    if (k == key) {
                        return (offset, i);
                    }
                }
            }

            return (null, -1);
        }

        protected override void Dispose(bool disposing) {
            StopAll();
            DiscardAllPreloadedSfx();
        }

        private static WaveFormat RequiredFormat => new WaveFormat(44100, 16, 2);

        private readonly AudioManager _audioManager;

        private readonly List<bool> _playingStates = new List<bool>();
        private readonly List<(string Key, WaveOffsetStream Offset, WaveStream ToOffset, WaveStream Source)> _playingWaveStreams = new List<(string, WaveOffsetStream, WaveStream, WaveStream)>();
        private readonly Dictionary<string, (byte[] Data, WaveFormat Format)> _preloaded = new Dictionary<string, (byte[] Data, WaveFormat Format)>();

        private readonly object _queueLock = new object();

    }
}
