using System.Drawing;

namespace OpenMLTD.MilliSim.Theater.Internal {
    internal struct NoteAnimationMetrics {

        public float GlobalSpeedScale { get; set; }

        public int TrackCount { get; set; }

        public float[] NoteStartXRatios { get; set; }

        public float[] NoteEndXRatios { get; set; }

        public Size ClientSize { get; set; }

        public float Top { get; set; }

        public float Bottom { get; set; }

    }
}
