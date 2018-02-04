using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NAudio.Wave;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Extensions;
using OpenMLTD.MilliSim.Plugin;
using SharpAL;
using SharpAL.Extensions;
using SharpAL.OpenAL;

namespace OpenMLTD.MilliSim.Audio {
    public sealed class AudioManager : DisposableBase {

        public AudioManager() {
            _device = new AudioDevice();
            _context = new AudioContext(_device);

            Activate();
        }

        public void Activate() {
            EnsureNotDisposed();

            _context.MakeCurrent();
        }

        [CanBeNull]
        public Sound FindSound([NotNull] string fileName) {
            return _loadedSounds.FirstOrDefault(s => s.FileName == fileName).Sound;
        }

        [NotNull, ItemNotNull]
        public Sound[] FindSounds([NotNull] string fileName) {
            return _loadedSounds.Where(s => s.FileName == fileName).Select(s => s.Sound).ToArray();
        }

        /// <summary>
        /// Load sound from file, with caching mechanism.
        /// </summary>
        /// <param name="fileName">Name of the sound file.</param>
        /// <param name="format">Audio format.</param>
        /// <returns>Loaded <see cref="Sound"/> object.</returns>
        public Sound LoadSound([NotNull] string fileName, [NotNull] IAudioFormat format) {
            var loadedArray = _loadedSounds.Where(t => t.FileName == fileName).Select(t => t.Sound).ToArray();

            if (loadedArray.Length == 0) {
                return LoadSoundDirect(fileName, format);
            }

            // Finds the first available (not in use) audio stream.
            Sound availableSound = null;
            foreach (var sound in loadedArray) {
                var source = sound.Source;
                var state = source.State;

                switch (state) {
                    case ALSourceState.Initial:
                    case ALSourceState.Stopped:
                        availableSound = sound;
                        break;
                    case ALSourceState.Playing:
                    case ALSourceState.Paused:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (availableSound != null) {
                    break;
                }
            }

            // If we are lucky...
            if (availableSound != null) {
                return availableSound;
            }

            // Or we have to clone and create one...
            availableSound = loadedArray[0];
            var newSound = LoadSoundDirect(fileName, availableSound.Data, availableSound.Format);
            // ... and copies its params.
            newSound.Source.Volume = availableSound.Source.Volume;

            return newSound;
        }

        public Sound LoadSoundDirect([NotNull] string fileName, [NotNull] IAudioFormat format) {
            using (var stream = format.Read(fileName)) {
                return LoadSoundDirect(fileName, stream);
            }
        }

        public Sound LoadSoundDirect([NotNull] string fileName, [NotNull] WaveStream stream) {
            WaveStream audioStream;
            var originalFormat = stream.WaveFormat;

            if (!AudioHelper.NeedsFormatConversionFrom(originalFormat, RequiredFormat)) {
                audioStream = stream;
            } else {
                // ResamplerDmoStream always works but it requires MediaFoundation.
                // So here we wrap the WdlResamplingSampleProvider to a stream, and voila!
                audioStream = new WdlResampling16Stream(stream, RequiredFormat.SampleRate);
            }

            var audioData = audioStream.ReadAll();

            if (audioStream != stream) {
                // TODO: Warning: this also destroys the internal 'stream'. So 'stream' must not be used again.
                audioStream.Dispose();
            }

            return LoadSoundDirect(fileName, audioData, originalFormat);
        }

        public void UnmanageSounds([NotNull] string fileName) {
            var count = _loadedSounds.Count;

            var indices = new List<int>();
            for (var i = 0; i < count; ++i) {
                if (_loadedSounds[i].FileName == fileName) {
                    indices.Add(i);
                }
            }

            var delta = 0;
            foreach (var index in indices) {
                _loadedSounds.RemoveAt(index - delta);
                ++delta;
            }
        }

        public void UnmanageSound([NotNull] string fileName) {
            var count = _loadedSounds.Count;

            var index = -1;
            for (var i = 0; i < count; ++i) {
                if (_loadedSounds[i].FileName == fileName) {
                    index = i;
                    break;
                }
            }

            if (index >= 0) {
                _loadedSounds.RemoveAt(index);
            }
        }

        public void UnmanageSound([NotNull] Sound sound) {
            var count = _loadedSounds.Count;

            var index = -1;
            for (var i = 0; i < count; ++i) {
                if (_loadedSounds[i].Sound == sound) {
                    index = i;
                    break;
                }
            }

            if (index >= 0) {
                _loadedSounds.RemoveAt(index);
            }
        }

        public IEnumerable<Sound> GetLoadedSounds() {
            return _loadedSounds.Select(t => t.Sound);
        }

        public IEnumerable<(string FileName, Sound sound)> GetLoadedEntries() {
            return _loadedSounds;
        }

        protected override void Dispose(bool disposing) {
            AudioContext.Reset();

            foreach (var (_, sound) in _loadedSounds) {
                sound.Dispose();
            }

            _context?.Dispose();
            _device?.Dispose();
        }

        private Sound LoadSoundDirect([NotNull] string fileName, [NotNull] byte[] data, [NotNull] WaveFormat format) {
            var buffer = new AudioBuffer(_context);
            buffer.BufferData(data, format.SampleRate);

            var source = new AudioSource(_context);

            source.Bind(buffer);

            var sound = new Sound(source, buffer, format);

            sound.Data = (byte[])data.Clone();

            _loadedSounds.Add((fileName, sound));

            return sound;
        }

        private static readonly WaveFormat RequiredFormat = new WaveFormat();

        private readonly AudioDevice _device;
        private readonly AudioContext _context;

        private readonly List<(string FileName, Sound Sound)> _loadedSounds = new List<(string FileName, Sound Sound)>();

    }
}
