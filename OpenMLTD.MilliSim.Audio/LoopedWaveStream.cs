using NAudio.Wave;

namespace OpenMLTD.MilliSim.Audio {
    internal sealed class LoopedWaveStream : WaveStream {

        internal LoopedWaveStream(WaveStream baseStream) {
            BaseStream = baseStream;
        }

        public WaveStream BaseStream { get; }

        public override WaveFormat WaveFormat => BaseStream.WaveFormat;

        public override int Read(byte[] buffer, int offset, int count) {
            var totalBytesRead = 0;
            var baseStream = BaseStream;

            while (totalBytesRead < count) {
                var read = baseStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);

                if (read == 0) {
                    if (baseStream.Position == 0) {
                        break;
                    }
                    baseStream.Position = 0;
                }

                totalBytesRead += read;
            }

            return totalBytesRead;
        }

        public override long Length => BaseStream.Length;

        public override long Position {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }

    }
}
