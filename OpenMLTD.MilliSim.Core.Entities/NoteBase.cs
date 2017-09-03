namespace OpenMLTD.MilliSim.Core.Entities {
    public abstract class NoteBase {

        /// <summary>
        /// Absolute time of this note, in seconds.
        /// </summary>
        public double AbsoluteTime { get; set; }

        /// <summary>
        /// Whether this note selected or not. (Used in editor?)
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Corresponding tick count of absolute time.
        /// </summary>
        public long Tick { get; set; }

        /// <summary>
        /// Index of the measure which this note is in.
        /// </summary>
        public int Measure { get; set; }

        /// <summary>
        /// Index of the beat which this note is on.
        /// </summary>
        public int Beat { get; set; }
        
        /// <summary>
        /// Index of the track which this note is on. See <see cref="NoteHelper.GetTrackTypeFromTrackIndex"/> for more information.
        /// </summary>
        public int TrackIndex { get; set; }

    }
}
