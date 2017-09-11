namespace OpenMLTD.MilliSim.Core.Entities.Runtime {
    public static class RuntimeNoteHelper {

        /// <summary>
        /// Convert ticks to seconds.
        /// </summary>
        /// <param name="ticks">Number of ticks.</param>
        /// <remarks>See remarks of <see cref="RuntimeNote.Ticks"/>.</remarks>
        /// <returns>Seconds.</returns>
        public static double TicksToSeconds(long ticks) {
            // Surprised?
            return (double)ticks / 1120;
        }

    }
}
