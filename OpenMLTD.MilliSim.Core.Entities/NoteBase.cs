using System;
using System.Runtime.Serialization;

namespace OpenMLTD.MilliSim.Core.Entities {
    [Serializable]
    [DataContract]
    public abstract class NoteBase {

        /// <summary>
        /// Absolute time of this note, in seconds.
        /// </summary>
        [DataMember(Name = "absoluteTime")]
        public double AbsoluteTime { get; set; }

        /// <summary>
        /// Whether this note selected or not. (Used in editor?)
        /// </summary>
        [DataMember(Name = "isSelected")]
        public bool IsSelected { get; set; }

        /// <summary>
        /// Corresponding tick count of absolute time.
        /// </summary>
        [DataMember(Name = "tick")]
        public long Tick { get; set; }

        /// <summary>
        /// Index of the measure which this note is in.
        /// </summary>
        [DataMember(Name = "measure")]
        public int Measure { get; set; }

        /// <summary>
        /// Index of the beat which this note is on.
        /// </summary>
        [DataMember(Name = "beat")]
        public int Beat { get; set; }

        /// <summary>
        /// Index of the track which this note is on. See <see cref="ScoreHelper.GetTrackTypeFromTrackIndex"/> for more information.
        /// </summary>
        [DataMember(Name = "trackIndex")]
        public int TrackIndex { get; set; }

    }
}
