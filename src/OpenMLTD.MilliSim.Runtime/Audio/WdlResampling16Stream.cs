using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace OpenMLTD.MilliSim.Audio {
    internal sealed class WdlResampling16Stream : WaveStream {

        internal WdlResampling16Stream([NotNull] WaveStream sourceStream, int sampleRate) {
            _sourceStream = sourceStream;
            _sourceSampleProvider = sourceStream.ToSampleProvider();
            _sampleProvider = new WdlResamplingSampleProvider(_sourceSampleProvider, sampleRate);
            // WdlResamplingSampleProvider always outputs IEEE float samples, so a 16-bit PCM wrapper is required.
            // See the source code of WdlResamplingSampleProvider in NAudio.
            // The wrapping technique is adapted from WaveFileWriter.CreateWaveFile16(), inspired from
            // Mark Heath's (creator of NAudio) article: http://markheath.net/post/fully-managed-input-driven-resampling-wdl.
            _to16Provider = new SampleToWaveProvider16(_sampleProvider);
        }

        public override int Read(byte[] buffer, int offset, int count) {
            return _to16Provider.Read(buffer, offset, count);
        }

        public override WaveFormat WaveFormat => _to16Provider.WaveFormat;

        public override long Length {
            get {
                var sourceLength = _sourceStream.Length;
                var ratio = GetScalingRatio();

                return (long)Math.Ceiling(sourceLength * ratio);
            }
        }

        public override long Position {
            get {
                var sourcePosition = _sourceStream.Position;
                var ratio = GetScalingRatio();
                var calculatedPosition = (long)Math.Round(sourcePosition * ratio);

                calculatedPosition += calculatedPosition % WaveFormat.BlockAlign;

                return calculatedPosition;
            }
            set {
                var ratio = GetScalingRatio();
                var calculatedPosition = (long)Math.Round(value / ratio);

                _sourceStream.Position = calculatedPosition;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetScalingRatio() {
            var sourceFormat = _sourceStream.WaveFormat;

            // 32-bit to 16-bit
            return (float)WaveFormat.SampleRate / sourceFormat.SampleRate / 2;
        }

        private readonly WaveStream _sourceStream;
        private readonly ISampleProvider _sourceSampleProvider;
        private readonly IWaveProvider _to16Provider;
        private readonly WdlResamplingSampleProvider _sampleProvider;

    }
}
