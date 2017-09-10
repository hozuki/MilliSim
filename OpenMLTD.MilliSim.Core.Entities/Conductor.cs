using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core.Entities {
    /// <summary>
    /// A reserved note type for changing music BPM.
    /// </summary>
    [Serializable]
    [DataContract]
    public sealed class Conductor : NoteBase {

        /// <summary>
        /// The new tempo.
        /// </summary>
        [DataMember(Name = "tempo")]
        public double Tempo { get; set; }

        /// <summary>
        /// The numerator of new measure signature.
        /// </summary>
        [DataMember(Name = "signatureNumerator")]
        public int SignatureNumerator { get; set; }

        /// <summary>
        /// The denominator of new measure signature.
        /// </summary>
        [DataMember(Name = "signatureDenominator")]
        public int SignatureDenominator { get; set; }

        /// <summary>
        /// Unknown.
        /// </summary>
        [NotNull]
        [DataMember(Name = "marker")]
        public string Marker { get; set; } = string.Empty;

    }
}
