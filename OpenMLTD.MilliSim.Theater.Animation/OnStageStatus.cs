namespace OpenMLTD.MilliSim.Theater.Animation {
    /// <summary>
    /// Three stages of a note's lifespan.
    /// </summary>
    public enum OnStageStatus {

        /// <summary>
        /// The note has not reached the stage.
        /// </summary>
        Incoming = -1,
        /// <summary>
        /// The note has entered the stage, being visible to the viewer.
        /// </summary>
        Visible = 0,
        /// <summary>
        /// The note has left the stage.
        /// </summary>
        Passed = 1

    }
}
