namespace OpenMLTD.MilliSim.Core.Entities {
    public enum NoteType {

        Tap = 0,
        Flick = 1,
        Hold = 2,
        Slide = 3,

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
        SpecialPrepare = 102

    }
}
