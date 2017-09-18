namespace OpenMLTD.MilliSim.Core {
    /// <summary>
    /// An interface providing API version information.
    /// </summary>
    public interface IApiVersionProvider {

        /// <summary>
        /// Gets exposed API version number.
        /// </summary>
        int ApiVersion { get; }

    }
}
