namespace OpenMLTD.MilliSim.Core.Entities {
    public sealed class PolyPoint {

        /// <summary>
        /// Delta tick compared to the main note. (?)
        /// </summary>
        public int Subtick { get; set; }
        
        /// <summary>
        /// X position of this polypoint.
        /// </summary>
        public float PositionX { get; set; }

    }
}
