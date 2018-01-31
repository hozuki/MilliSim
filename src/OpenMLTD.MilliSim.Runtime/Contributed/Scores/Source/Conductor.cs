namespace OpenMLTD.MilliSim.Contributed.Scores.Source {
    /// <inheritdoc cref="NoteBase"/>
    /// <summary>
    /// A reserved note type for changing music BPM.
    /// </summary>
    public class Conductor : NoteBase {

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

    }
}
