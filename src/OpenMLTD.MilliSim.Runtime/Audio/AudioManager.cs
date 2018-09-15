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
    /// <inheritdoc />
    /// <summary>
    /// A basic audio manager.
    /// </summary>
    /// <remarks>
    /// <see cref="AudioManager"/> has a caching and disposing mechanism. Loaded sounds will be disposed when
    /// the this class is disposed. To disable this behavior, you must call <see cref="UnmanageSound(Sound)"/>,
    /// <see cref="UnmanageSound(string)"/> or <see cref="UnmanageSounds(string)"/> on the expected sound(s) explicitly.
    /// </remarks>
    public class AudioManager : DisposableBase {

        /// <summary>
        /// Creates a new <see cref="AudioManager"/> instance.
        /// </summary>
        public AudioManager() {
            _device = new AudioDevice();
            _context = new AudioContext(_device);

            Activate();
        }

        /// <summary>
        /// Activates this audio manager.
        /// </summary>
        public void Activate() {
            EnsureNotDisposed();

            _context.MakeCurrent();
        }

        /// <summary>
        /// Find the first <see cref="Sound"/> whose file name is the specified file name.
        /// </summary>
        /// <param name="fileName">File name of the sound. Case sensitive.</param>
        /// <returns>Found <see cref="Sound"/> or <see langword="null"/> if no <see cref="Sound"/> is found.</returns>
        [CanBeNull]
        public Sound FindSound([NotNull] string fileName) {
            foreach (var entry in _loadedEntries) {
                if (entry.FileName == fileName) {
                    return entry.Sound;
                }
            }

            return null;
        }

        /// <summary>
        /// Find <see cref="Sound"/>s whose file names are the specified file name.
        /// </summary>
        /// <param name="fileName">File name of the sound. Case sensitive.</param>
        /// <returns>Found <see cref="Sound"/>s.</returns>
        [NotNull, ItemNotNull]
        public Sound[] FindSounds([NotNull] string fileName) {
            var sounds = new List<Sound>();

            foreach (var entry in _loadedEntries) {
                if (entry.FileName == fileName) {
                    sounds.Add(entry.Sound);
                }
            }

            return sounds.ToArray();
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

            foreach (var it in _loadedEntries) {
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

        /// <summary>
        /// Loads a sound directly.
        /// </summary>
        /// <param name="fileName">File name of the sound.</param>
        /// <param name="format">An <see cref="IAudioFormat"/> that provides wave data stream.</param>
        /// <param name="manageResult">Whether the result should be managed (and cached) or not. If the sound is not managed, you must dispose it manually.</param>
        /// <returns>Loaded <see cref="Sound"/> object.</returns>
        [NotNull]
        public Sound LoadSoundDirect([NotNull] string fileName, [NotNull] IAudioFormat format, bool manageResult) {
            using (var stream = format.Read(fileName)) {
                return LoadSoundDirect(fileName, stream, manageResult);
            }
        }

        /// <summary>
        /// Loads a sound directly.
        /// </summary>
        /// <param name="fileName">File name of the sound.</param>
        /// <param name="stream">Sound wave data stream.</param>
        /// <param name="manageResult">Whether the result should be managed (and cached) or not. If the sound is not managed, you must dispose it manually.</param>
        /// <returns>Loaded <see cref="Sound"/> object.</returns>
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

        /// <summary>
        /// Unmanage all sounds with the given file name.
        /// </summary>
        /// <param name="fileName">The file name of the sounds.</param>
        public void UnmanageSounds([NotNull] string fileName) {
            var count = _loadedEntries.Count;

            var indices = new List<int>();
            for (var i = 0; i < count; ++i) {
                if (_loadedEntries[i].FileName == fileName) {
                    indices.Add(i);
                }
            }

            var delta = 0;
            foreach (var index in indices) {
                _loadedEntries.RemoveAt(index - delta);
                ++delta;
            }
        }

        /// <summary>
        /// Unmanage the first sound with the given file name.
        /// </summary>
        /// <param name="fileName">The file name of the sound.</param>
        public void UnmanageSound([NotNull] string fileName) {
            var count = _loadedEntries.Count;

            var index = -1;

            for (var i = 0; i < count; ++i) {
                if (_loadedEntries[i].FileName == fileName) {
                    index = i;
                    break;
                }
            }

            if (index >= 0) {
                _loadedEntries.RemoveAt(index);
            }
        }

        /// <summary>
        /// Unmanage a specified <see cref="Sound"/> instance.
        /// </summary>
        /// <param name="sound">The <see cref="Sound"/> to unmanage.</param>
        public void UnmanageSound([NotNull] Sound sound) {
            var count = _loadedEntries.Count;

            var index = -1;

            for (var i = 0; i < count; ++i) {
                if (_loadedEntries[i].Sound == sound) {
                    index = i;
                    break;
                }
            }

            if (index >= 0) {
                _loadedEntries.RemoveAt(index);
            }
        }

        /// <summary>
        /// Gets all loaded sounds.
        /// </summary>
        /// <returns>All loaded sounds.</returns>
        [NotNull, ItemNotNull]
        public IEnumerable<Sound> GetLoadedSounds() {
            return _loadedEntries.Select(t => t.Sound);
        }

        /// <summary>
        /// Gets all loaded entries.
        /// </summary>
        /// <returns>All loaded entries (tuple of <see cref="string"/> and <see cref="Sound"/>).</returns>
        [NotNull]
        public IEnumerable<(string FileName, Sound sound)> GetLoadedEntries() {
            return _loadedEntries;
        }

        protected override void Dispose(bool disposing) {
            AudioContext.Reset();

            foreach (var (_, sound) in _loadedEntries) {
                sound.Dispose();
            }

            _context?.Dispose();
            _device?.Dispose();
        }

        private bool HasLoadedFile([NotNull] string fileName) {
            foreach (var it in _loadedEntries) {
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

                _loadedEntries.Add((fileName, sound));
            }

            return sound;
        }

        // Required format: PCM 44.1 kHz stereo 16-bit signed integer
        private static readonly WaveFormat RequiredFormat = new WaveFormat();

        [NotNull]
        private readonly AudioDevice _device;
        [NotNull]
        private readonly AudioContext _context;

        [NotNull]
        private readonly List<(string FileName, Sound Sound)> _loadedEntries = new List<(string FileName, Sound Sound)>();

    }
}
