using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core.Entities.Extensions {
    public static class NoteBaseExtensions {

        public static TrackType GetTrackType([NotNull] this NoteBase note) {
            return NoteHelper.GetTrackTypeFromTrackIndex(note.TrackIndex);
        }

    }
}
