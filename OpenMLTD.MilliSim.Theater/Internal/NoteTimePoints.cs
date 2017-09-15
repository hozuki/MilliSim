namespace OpenMLTD.MilliSim.Theater.Internal {
    public struct NoteTimePoints {

        public NoteTimePoints(double enter, double leave) {
            Enter = enter;
            Leave = leave;
        }

        /// <summary>
        /// Gets/sets the time when this note enters the stage, in seconds.
        /// </summary>
        public double Enter { get; set; }

        /// <summary>
        /// Gets/sets the time when this note leaves the stage, in seconds.
        /// </summary>
        public double Leave { get; set; }

        /// <summary>
        /// Gets the duration of this note appearing on stage, in seconds.
        /// </summary>
        public double Duration => Leave - Enter;

    }
}
