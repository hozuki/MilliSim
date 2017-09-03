using System;

namespace OpenMLTD.MilliSim.Core.Entities {
    public static class NoteHelper {

        public static TrackType GetTrackTypeFromTrackIndex(int trackIndex) {
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

    }
}
