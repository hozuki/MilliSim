using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using NAudio.Wave;
using OpenMLTD.MilliSim.Audio.Extending;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Extensions;
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
        [NotNull]
        public Sound LoadSound([NotNull] string fileName, [NotNull] IAudioFormat format) {
            return LoadSound(fileName, format, true);
        }

        /// <summary>
        /// Load sound from file, with optional caching mechanism.
        /// </summary>
        /// <param name="fileName">Name of the sound file.</param>
        /// <param name="format">Audio format.</param>
        /// <param name="manageResult">Should the result be managed. A managed <see cref="Sound"/> can be reused and queried.</param>
        /// <returns>Loaded <see cref="Sound"/> object.</returns>
        [NotNull]
        public Sound LoadSound([NotNull] string fileName, [NotNull] IAudioFormat format, bool manageResult) {
            if (!HasLoadedFile(fileName)) {
                return LoadSoundDirect(fileName, format, manageResult);
            }

            // Finds the first available (not in use) audio stream.
            Sound availableSound = null;
            Sound firstSound = null;

            foreach (var it in _loadedSounds) {
                if (it.FileName != fileName) {
                    continue;
                }

                var source = it.Sound.Source;
                var state = source.State;

                if (firstSound == null) {
                    firstSound = it.Sound;
                }

                switch (state) {
                    case ALSourceState.Initial:
                    case ALSourceState.Stopped:
                        availableSound = it.Sound;
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
            availableSound = firstSound;

            Debug.Assert(availableSound != null, nameof(availableSound) + " != null");

            var newSound = LoadSoundDirect(fileName, availableSound.Data, availableSound.Format, false, manageResult);

            // ... and copies its params.
            newSound.Source.Volume = availableSound.Source.Volume;

            return newSound;
        }

        [NotNull]
        public Sound LoadSoundDirect([NotNull] string fileName, [NotNull] IAudioFormat format, bool manageResult) {
            using (var stream = format.Read(fileName)) {
                return LoadSoundDirect(fileName, stream, manageResult);
            }
        }

        [NotNull]
        public Sound LoadSoundDirect([NotNull] string fileName, [NotNull] WaveStream stream, bool manageResult) {
            WaveStream audioStream;
            var originalFormat = stream.WaveFormat;

            if (!AudioHelper.NeedsFormatConversionFrom(originalFormat, RequiredFormat)) {
                audioStream = stream;
            } else {
                // ResamplerDmoStream always works but it requires MediaFoundation.
                // So here we wrap the WdlResamplingSampleProvider to a stream, and voila!
                audioStream = new WdlResampling16Stream(stream, RequiredFormat.SampleRate);
            }

            var loadedFormat = audioStream.WaveFormat;
            var audioData = audioStream.ReadAll();

            if (audioStream != stream) {
                // TODO: Warning: this also destroys the internal 'stream'. So 'stream' must not be used again.
                audioStream.Dispose();
            }

            return LoadSoundDirect(fileName, audioData, loadedFormat, false, manageResult);
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

        [NotNull, ItemNotNull]
        public IEnumerable<Sound> GetLoadedSounds() {
            return _loadedSounds.Select(t => t.Sound);
        }

        [NotNull]
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

        private bool HasLoadedFile([NotNull] string fileName) {
            foreach (var it in _loadedSounds) {
                if (it.FileName == fileName) {
                    return true;
                }
            }

            return false;
        }

        [NotNull]
        private Sound LoadSoundDirect([NotNull] string fileName, [NotNull] byte[] data, [NotNull] WaveFormat format, bool cloneData, bool manageResult) {
            var buffer = new AudioBuffer(_context);

            buffer.BufferData(data, format.SampleRate);

            var source = new AudioSource(_context);

            source.Bind(buffer);

            var sound = new Sound(source, buffer, format);

            if (manageResult) {
                if (cloneData) {
                    sound.Data = (byte[])data.Clone();
                } else {
                    sound.Data = data;
                }

                _loadedSounds.Add((fileName, sound));
            }

            return sound;
        }

        private static readonly WaveFormat RequiredFormat = new WaveFormat();

        [NotNull]
        private readonly AudioDevice _device;
        [NotNull]
        private readonly AudioContext _context;

        [NotNull]
        private readonly List<(string FileName, Sound Sound)> _loadedSounds = new List<(string FileName, Sound Sound)>();

    }
}
