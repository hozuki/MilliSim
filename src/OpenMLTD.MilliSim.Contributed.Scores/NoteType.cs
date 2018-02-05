namespace OpenMLTD.MilliSim.Contributed.Scores {
    public enum NoteType {

        Invalid = 0,

        Tap = 1,
        Flick = 2,
        Hold = 3,
        Slide = 4,

        /// <summary>
        /// The Special note.
        /// </summary>
        Special = 100,
        /// <summary>
        /// A virtual note representing the end of Special period.
        /// </summary>
        SpecialEnd = 101,
        /// <summary>
        /// A virtual note representing the start of tap points animation.
        /// </summary>
        SpecialPrepare = 102,
        /// <summary>
        /// The user should prepare to play the score. It is usually 1.5 seconds before the first playable note.
        /// The tap points will respond to this type of note and play "fade-in" animation.
        /// </summary>
        ScorePrepare = 103

    }
}
