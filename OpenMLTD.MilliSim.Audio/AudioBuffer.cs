using System;
using JetBrains.Annotations;
using NAudio.Wave;
using OpenTK.Audio.OpenAL;

namespace OpenMLTD.MilliSim.Audio {
    public sealed class AudioBuffer : AudioObject {

        public AudioBuffer([NotNull] AudioContext context) {
            Context = context;
            Alc.MakeContextCurrent(context.NativeContext);
            AL.GenBuffer(out _buffer);
        }

        public AudioContext Context { get; }

        /// <summary>
        /// Loads audio data from a <see cref="WaveStream"/>.
        /// The format should be stereo, 16-bit unsigned integer, PCM encoded.
        /// If not, a <see cref="ResamplerDmoStream"/> will be used to convert the original stream to the standard format.
        /// </summary>
        /// <param name="stream">The <see cref="WaveStream"/> that contains expected audio data.</param>
        public void LoadData([NotNull] WaveStream stream) {
            WaveStream audioStream;

            var originalFormat = stream.WaveFormat;
            if (!AudioHelper.NeedsFormatConversionFrom(originalFormat, RequiredFormat)) {
                audioStream = stream;
            } else {
                // TODO: this one uses Media Foundation API.
                audioStream = new ResamplerDmoStream(stream, RequiredFormat);
            }

            var dataLength = audioStream.Length;
            var data = new byte[dataLength];

            var offset = 0;
            var block = new byte[4096];
            var read = 1;
            while (read > 0) {
                read = audioStream.Read(block, 0, block.Length);

                Array.Copy(block, 0, data, offset, read);
                offset += read;

                if (read < block.Length) {
                    break;
                }
            }

            if (audioStream != stream) {
                audioStream.Dispose();
            }

            LoadData(data, audioStream.WaveFormat.SampleRate);
        }

        /// <summary>
        /// Loads audio data to this buffer. The data must be stereo, 16-bit unsigned integer.
        /// </summary>
        /// <param name="data">Data bytes.</param>
        /// <param name="sampleRate">Sample rate.</param>
        internal void LoadData([NotNull] byte[] data, int sampleRate) {
            var nb = unchecked((int)NativeBuffer);
            AL.BufferData(nb, ALFormat.Stereo16, data, data.Length, sampleRate);

            Data = data;
            SampleRate = sampleRate;
        }

        internal int SampleRate { get; private set; }

        internal byte[] Data { get; private set; }

        internal uint NativeBuffer => _buffer;

        protected override void Dispose(bool disposing) {
            AL.DeleteBuffer(ref _buffer);
        }

        private static readonly WaveFormat RequiredFormat = new WaveFormat();

        private uint _buffer;

    }
}
