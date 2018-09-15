namespace OpenMLTD.MilliSim.Graphics {
    /// <summary>
    /// Graphics backend.
    /// </summary>
    public enum GraphicsBackend {

        /// <summary>
        /// Unknown backend.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Direct3D 11.
        /// </summary>
        Direct3D11 = 1,
        /// <summary>
        /// OpenGL 3.0+.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        OpenGL = 2

    }
}
