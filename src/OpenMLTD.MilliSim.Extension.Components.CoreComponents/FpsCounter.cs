using System.Collections.Generic;
using System.Linq;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    internal sealed class FpsCounter {

        public long TotalFrames { get; private set; }

        public float TotalSeconds { get; private set; }

        public float Current { get; private set; }

        public float Average { get; private set; }

        public void Update(float deltaTime) {
            Current = 1.0f / deltaTime;

            _sampleBuffer.Enqueue(Current);

            if (_sampleBuffer.Count > MaximumSamples) {
                _sampleBuffer.Dequeue();
                Average = _sampleBuffer.Average(i => i);
            } else {
                Average = Current;
            }

            TotalFrames++;
            TotalSeconds += deltaTime;
        }

        private readonly Queue<float> _sampleBuffer = new Queue<float>();

        private const int MaximumSamples = 100;

    }
}
