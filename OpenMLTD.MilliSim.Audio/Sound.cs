using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenTK.Audio.OpenAL;

namespace OpenMLTD.MilliSim.Audio {
    public sealed class Sound : DisposableBase {

        internal Sound([NotNull] AudioSource source, [NotNull] AudioBuffer buffer) {
            _source = source;
            _buffer = buffer;
        }

        public AudioSource Source => _source;

        public AudioBuffer Buffer => _buffer;

        protected override void Dispose(bool disposing) {
            if (_source != null) {
                _source.Stop();

                AL.BindBufferToSource(_source.NativeSource, 0);
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
