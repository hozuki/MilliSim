using JetBrains.Annotations;
using NAudio.Wave;
using OpenMLTD.MilliSim.Core;
using SharpAL;
using SharpAL.Extensions;

namespace OpenMLTD.MilliSim.Audio {
    /// <inheritdoc />
    /// <summary>
    /// Sound object which contains audio data, audio source.
    /// </summary>
    public sealed class Sound : DisposableBase {

        internal Sound([NotNull] AudioSource source, [NotNull] AudioBuffer buffer, [NotNull] WaveFormat format) {
            _source = source;
            _buffer = buffer;
            Format = format;
        }

        /// <summary>
        /// Gets the audio source object.
        /// </summary>
        [CanBeNull]
        public AudioSource Source => _source;

        /// <summary>
        /// Gets the audio buffer object.
        /// </summary>
        [CanBeNull]
        public AudioBuffer Buffer => _buffer;

        /// <summary>
        /// Gets the wave format of this <see cref="Sound"/>.
        /// </summary>
        [NotNull]
        public WaveFormat Format { get; }

        /// <summary>
        /// Gets or sets the raw audio data.
        /// </summary>
        [NotNull]
        internal byte[] Data { get; set; } = EmptyBytes;

        protected override void Dispose(bool disposing) {
            if (_source != null) {
                _source.Stop();
                _source.Bind(null);
            }

            _source?.Dispose();
            _buffer?.Dispose();

            _source = null;
            _buffer = null;
        }

        private static readonly byte[] EmptyBytes = new byte[0];

        private AudioSource _source;
        private AudioBuffer _buffer;

    }
}
