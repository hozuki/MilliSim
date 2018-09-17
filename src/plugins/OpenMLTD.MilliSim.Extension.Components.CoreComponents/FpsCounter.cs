using System.Collections.Generic;
using System.Linq;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    /// <summary>
    /// A simple frame rate counter.
    /// </summary>
    internal sealed class FpsCounter {

        /// <summary>
        /// Gets the total number of frames counted.
        /// </summary>
        public long TotalFrames { get; private set; }

        /// <summary>
        /// Gets the total seconds of time elapsed.
        /// </summary>
        public float TotalSeconds { get; private set; }

        /// <summary>
        /// Gets current frame rate.
        /// </summary>
        public float Current { get; private set; }

        /// <summary>
        /// Gets the average frame rate.
        /// </summary>
        public float Average { get; private set; }

        /// <summary>
        /// Triggers and update and recompute all the values.
        /// </summary>
        /// <param name="deltaTime">Time elapsed from last frame to current frame, in seconds.</param>
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
