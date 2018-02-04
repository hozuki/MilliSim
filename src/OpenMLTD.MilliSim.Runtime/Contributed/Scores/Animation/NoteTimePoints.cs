namespace OpenMLTD.MilliSim.Contributed.Scores.Animation {
    public struct NoteTimePoints {

        public NoteTimePoints(float enter, float leave) {
            Enter = enter;
            Leave = leave;
        }

        /// <summary>
        /// Gets/sets the time when this note enters the stage, in seconds.
        /// </summary>
        public float Enter { get; }

        /// <summary>
        /// Gets/sets the time when this note leaves the stage, in seconds.
        /// </summary>
        public float Leave { get; }

        /// <summary>
        /// Gets the duration of this note appearing on stage, in seconds.
        /// </summary>
        public float Duration => Leave - Enter;

    }
}
