using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    /// <summary>
    /// Synchronization method of <see cref="SyncTimer"/>.
    /// </summary>
    public enum TimerSyncMethod {

        /// <summary>
        /// Uses the naive method. Synchronized time is directly read from time source.
        /// </summary>
        Naive = 0,
        /// <summary>
        /// Uses the estimated method. Estimates and calibrates the time by using both the time source and an internal <see cref="System.Diagnostics.Stopwatch"/>.
        /// </summary>
        [UsedImplicitly]
        Estimated = 1

    }
}
