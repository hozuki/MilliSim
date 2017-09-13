using NAudio.Wave;

namespace OpenMLTD.MilliSim.Audio {
    internal sealed class LoopedWaveStream : WaveStream {

        internal LoopedWaveStream(WaveStream baseStream, int maxLoops) {
            BaseStream = baseStream;
            _maxLoops = maxLoops;
        }

        public WaveStream BaseStream { get; }

        public override WaveFormat WaveFormat => BaseStream.WaveFormat;

        public override int Read(byte[] buffer, int offset, int count) {
            var totalBytesRead = 0;
            var baseStream = BaseStream;

            while (totalBytesRead < count) {
                var read = baseStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);

                if (read < count - totalBytesRead) {
                    if (read == 0 && baseStream.Position == 0) {
                        // Errored.
                        break;
                    }
                    baseStream.Position = 0;
                }

                totalBytesRead += read;
            }

            return totalBytesRead;
        }

        public override long Length => BaseStream.Length * _maxLoops;

        public override long Position {
            get => BaseStream.Position;
            set => BaseStream.Position = value % Length;
        }

        internal static readonly int DefaultMaxLoops = 100;

        private readonly int _maxLoops;

    }
}
