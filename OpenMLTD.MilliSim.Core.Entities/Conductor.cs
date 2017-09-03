using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core.Entities {
    /// <summary>
    /// A reserved note type for changing music BPM.
    /// </summary>
    public sealed class Conductor : NoteBase {

        /// <summary>
        /// The new tempo.
        /// </summary>
        public double Tempo { get; set; }

        /// <summary>
        /// The numerator of new measure signature.
        /// </summary>
        public int SignatureNumerator { get; set; }

        /// <summary>
        /// The denominator of new measure signature.
        /// </summary>
        public int SignatureDenominator { get; set; }

        /// <summary>
        /// Unknown.
        /// </summary>
        [NotNull]
        public string Marker { get; set; } = string.Empty;

    }
}
