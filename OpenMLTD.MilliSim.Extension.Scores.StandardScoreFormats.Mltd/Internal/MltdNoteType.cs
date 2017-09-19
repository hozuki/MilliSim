namespace OpenMLTD.MilliSim.Extension.Scores.StandardScoreFormats.Mltd.Internal {
    /// <summary>
    /// Note base types.
    /// </summary>
    /// <remarks>Reference: https://github.com/thiEFcat/ScrObjAnalyzer</remarks>
    internal enum MltdNoteType {

        SecondaryBeat = -2,
        PrimaryBeat = -1,
        TapSmall = 0,
        TapLarge = 1,
        FlickLeft = 2,
        FlickUp = 3,
        FlickRight = 4,
        HoldSmall = 5,
        SlideSmall = 6,
        HoldLarge = 7,
        Special = 8

    }
}
