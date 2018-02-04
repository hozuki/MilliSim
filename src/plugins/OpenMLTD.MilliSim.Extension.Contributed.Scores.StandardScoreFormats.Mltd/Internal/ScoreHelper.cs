using System;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.Mltd.Internal {
    internal static class ScoreHelper {

        internal static TrackType GetTrackTypeFromTrackIndex(int trackIndex) {
            /*
                -1                        (block data)  
                0                         (conductor data)  
                1, 2                      2Mix  
                3, 4                      2Mix+  
                9, 10, 11, 12             4Mix  
                25, 26, 27, 28, 29, 30	  6Mix  
                31, 32, 33, 34, 35, 36	  MillionMix
            */
            switch (trackIndex) {
                case -1:
                    return TrackType.Block;
                case 0:
                    return TrackType.Conductor;
                case 1:
                case 2:
                    return TrackType.D2Mix;
                case 3:
                case 4:
                    return TrackType.D2MixPlus;
                case 9:
                case 10:
                case 11:
                case 12:
                    return TrackType.D4Mix;
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                    return TrackType.D6Mix;
                case 31:
                case 32:
                case 33:
                case 34:
                case 35:
                case 36:
                    return TrackType.MillionMix;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackIndex), trackIndex, null);
            }
        }

        internal static int[] GetTrackIndicesFromTrackType(TrackType trackType) {
            switch (trackType) {
                case TrackType.Block:
                    return TracksList[0];
                case TrackType.Conductor:
                    return TracksList[1];
                case TrackType.D2Mix:
                    return TracksList[2];
                case TrackType.D2MixPlus:
                    return TracksList[3];
                case TrackType.D4Mix:
                    return TracksList[4];
                case TrackType.D6Mix:
                    return TracksList[5];
                case TrackType.MillionMix:
                    return TracksList[6];
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackType), trackType, null);
            }
        }

        internal static TrackType MapDifficultyToTrackType(Difficulty difficulty) {
            switch (difficulty) {
                case Difficulty.D2Mix:
                    return TrackType.D2Mix;
                case Difficulty.D2MixPlus:
                    return TrackType.D2MixPlus;
                case Difficulty.D4Mix:
                    return TrackType.D4Mix;
                case Difficulty.D6Mix:
                    return TrackType.D6Mix;
                case Difficulty.MillionMix:
                    return TrackType.MillionMix;
                default:
                    throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null);
            }
        }

        private static readonly int[][] TracksList = {
            new[] { -1 },
            new[] { 0 },
            new[] { 1, 2 },
            new[] { 3, 4 },
            new[] { 9, 10, 11, 12 },
            new[] { 25, 26, 27, 28, 29, 30 },
            new[] { 31, 32, 33, 34, 35, 36 }
        };

    }
}
