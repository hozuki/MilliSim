using System;

namespace OpenMLTD.MilliSim.Extension.Scores.StandardScoreFormats.Mltd.Internal {
    internal static class TrackHelper {

        internal static int GetTrackCount(Difficulty difficulty) {
            switch (difficulty) {
                case Difficulty.D2Mix:
                case Difficulty.D2MixPlus:
                    return 2;
                case Difficulty.D4Mix:
                    return 4;
                case Difficulty.D6Mix:
                case Difficulty.MillionMix:
                    return 6;
                default:
                    throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null);
            }
        }

    }
}
