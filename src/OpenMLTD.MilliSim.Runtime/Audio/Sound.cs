using JetBrains.Annotations;
using NAudio.Wave;
using OpenMLTD.MilliSim.Core;
using SharpAL;
using SharpAL.Extensions;

namespace OpenMLTD.MilliSim.Audio {
    public sealed class Sound : DisposableBase {

        internal Sound([NotNull] AudioSource source, [NotNull] AudioBuffer buffer, [NotNull] WaveFormat format) {
            _source = source;
            _buffer = buffer;
            Format = format;
        }

        public AudioSource Source => _source;

        public AudioBuffer Buffer => _buffer;

        public WaveFormat Format { get; }

        internal byte[] Data { get; set; }

        protected override void Dispose(bool disposing) {
            if (_source != null) {
                _source.Stop();

                Source.Bind(null);
            }

            _source?.Dispose();
            _buffer?.Dispose();

            _source = null;
            _buffer = null;
        }

        private AudioSource _source;
        private AudioBuffer _buffer;

    }
}
