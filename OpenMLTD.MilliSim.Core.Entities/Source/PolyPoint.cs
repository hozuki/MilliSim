using OpenMLTD.MilliSim.Core.Entities.Extending;

namespace OpenMLTD.MilliSim.Core.Entities.Source {
    /// <summary>
    /// A point following a hold or slide note.
    /// </summary>
    public class PolyPoint {

        /// <summary>
        /// Delta tick compared to the main note.
        /// </summary>
        public int Subtick { get; set; }

        /// <summary>
        /// Absolute X position of this <see cref="PolyPoint"/>. This indicates the starting position of next <see cref="PolyPoint"/>.
        /// Browse a <see cref="IScoreCompiler"/> implementation to see how to use this field.
        /// </summary>
        public float PositionX { get; set; }

    }
}
