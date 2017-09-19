using OpenMLTD.MilliSim.Core.Entities.Runtime;

namespace OpenMLTD.MilliSim.Core.Entities.Extensions {
    public static class RuntimeNoteExtensions {

        public static bool IsSync(this RuntimeNote note) {
            return note.PrevSync != null || note.NextSync != null;
        }

        public static bool HasPrevSync(this RuntimeNote note) {
            return note.PrevSync != null;
        }

        public static bool HasNextSync(this RuntimeNote note) {
            return note.NextSync != null;
        }

        public static bool IsHold(this RuntimeNote note) {
            return note.PrevHold != null || note.NextHold != null;
        }

        public static bool IsHoldStart(this RuntimeNote note) {
            return note.PrevHold == null && note.NextHold != null;
        }

        public static bool IsHoldMiddle(this RuntimeNote note) {
            return note.PrevHold != null && note.NextHold != null;
        }

        public static bool IsHoldEnd(this RuntimeNote note) {
            return note.PrevHold != null && note.NextHold == null;
        }

        public static bool HasPrevHold(this RuntimeNote note) {
            return note.PrevHold != null;
        }

        public static bool HasNextHold(this RuntimeNote note) {
            return note.NextHold != null;
        }

        public static bool IsFlick(this RuntimeNote note) {
            return note.PrevFlick != null || note.NextFlick != null;
        }

        public static bool IsFlickStart(this RuntimeNote note) {
            return note.PrevFlick == null && note.NextFlick != null;
        }

        public static bool IsFlickMiddle(this RuntimeNote note) {
            return note.PrevFlick != null && note.NextFlick != null;
        }

        public static bool IsFlickEnd(this RuntimeNote note) {
            return note.PrevFlick != null && note.NextFlick == null;
        }

        public static bool HasPrevFlick(this RuntimeNote note) {
            return note.PrevFlick != null;
        }

        public static bool HasNextFlick(this RuntimeNote note) {
            return note.NextFlick != null;
        }

        public static bool IsSlide(this RuntimeNote note) {
            return note.PrevSlide != null || note.NextSlide != null;
        }

        public static bool IsSlideStart(this RuntimeNote note) {
            return note.PrevSlide == null && note.NextSlide != null;
        }

        public static bool IsSlideMiddle(this RuntimeNote note) {
            return note.PrevSlide != null && note.NextSlide != null;
        }

        public static bool IsSlideEnd(this RuntimeNote note) {
            return note.PrevSlide != null && note.NextSlide == null;
        }

        public static bool HasPrevSlide(this RuntimeNote note) {
            return note.PrevSlide != null;
        }

        public static bool HasNextSlide(this RuntimeNote note) {
            return note.NextSlide != null;
        }

    }
}
