using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NAudio.Wave;
using OpenMLTD.MilliSim.Audio.Extending;
using OpenMLTD.MilliSim.Foundation;
using OpenTK.Audio.OpenAL;

namespace OpenMLTD.MilliSim.Audio {
    public sealed class AudioManager : AudioManagerBase {

        public AudioManager() {
            _device = new AudioDevice();
            _context = new AudioContext(_device);

            Activate();
        }

        public void Activate() {
            EnsureNotDisposed();

            _context.SetAsCurrent();
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
                    case AudioState.Loaded:
                    case AudioState.Stopped:
                        availableSound = sound;
                        break;
                    case AudioState.Playing:
                    case AudioState.Paused:
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
            var newSound = LoadSoundDirect(fileName, availableSound.Buffer.Data, availableSound.Buffer.SampleRate);
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
            var buffer = new AudioBuffer(_context);
            buffer.LoadData(stream);

            var source = new AudioSource(_context);

            AL.BindBufferToSource(source.NativeSource, buffer.NativeBuffer);

            var sound = new Sound(source, buffer);
            _loadedSounds.Add((fileName, sound));

            return sound;
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

        protected override void Dispose(bool disposing) {
            AudioContext.Reset();

            foreach (var (_, sound) in _loadedSounds) {
                sound.Dispose();
            }

            _context?.Dispose();
            _device?.Dispose();
        }

        private Sound LoadSoundDirect([NotNull] string fileName, [NotNull] byte[] data, int sampleRate) {
            var buffer = new AudioBuffer(_context);
            buffer.LoadData(data, sampleRate);

            var source = new AudioSource(_context);

            AL.BindBufferToSource(source.NativeSource, buffer.NativeBuffer);

            var sound = new Sound(source, buffer);
            _loadedSounds.Add((fileName, sound));

            return sound;
        }

        private readonly AudioDevice _device;
        private readonly AudioContext _context;

        private readonly List<(string FileName, Sound Sound)> _loadedSounds = new List<(string FileName, Sound Sound)>();

    }
}
