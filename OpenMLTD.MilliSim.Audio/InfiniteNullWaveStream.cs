using System;
using NAudio.Wave;

namespace OpenMLTD.MilliSim.Audio {
    internal sealed class InfiniteNullWaveStream : WaveStream {

        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override int Read(byte[] buffer, int offset, int count) {
            Array.Clear(buffer, offset, count);
            return count;
        }

        public override WaveFormat WaveFormat => Format;

        public override long Length => long.MaxValue;

        public override long Position { get; set; }

        public static readonly InfiniteNullWaveStream Instance = new InfiniteNullWaveStream();

        private InfiniteNullWaveStream() {
        }

        private static readonly WaveFormat Format = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);

    }
}
