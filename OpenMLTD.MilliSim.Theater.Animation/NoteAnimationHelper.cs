using OpenMLTD.MilliSim.Core.Entities.Runtime;

namespace OpenMLTD.MilliSim.Theater.Animation {
    public static class NoteAnimationHelper {

        public static NoteTimePoints CalculateNoteTimePoints(RuntimeNote note, float globalSpeedScale) {
            // Empirical formula: s = pow(game_setting, 3) * pow(note_speed, 2)
            var relativeSpeed = note.RelativeSpeed;
            var absoluteSpeed = globalSpeedScale * globalSpeedScale * globalSpeedScale * relativeSpeed * relativeSpeed;
            var leadTimeScaled = (float)note.LeadTime / absoluteSpeed;
            return new NoteTimePoints(note.HitTime - leadTimeScaled, note.HitTime);
        }

        public static NoteTimePoints CalculateNoteTimePoints(RuntimeNote note, NoteAnimationMetrics metrics) {
            return CalculateNoteTimePoints(note, metrics.GlobalSpeedScale);
        }

        public static OnStageStatus GetOnStageStatusOf(RuntimeNote note, double now, float globalSpeedScale) {
            var timePoints = CalculateNoteTimePoints(note, globalSpeedScale);
            return GetOnStageStatusOf(note, now, timePoints);
        }

        public static OnStageStatus GetOnStageStatusOf(RuntimeNote note, double now, NoteAnimationMetrics metrics) {
            var timePoints = CalculateNoteTimePoints(note, metrics);
            return GetOnStageStatusOf(note, now, timePoints);
        }

        public static OnStageStatus GetOnStageStatusOf(RuntimeNote note, double now, NoteTimePoints timePoints) {
            if (now < timePoints.Enter) {
                return OnStageStatus.Incoming;
            } else if (now > timePoints.Leave) {
                return OnStageStatus.Passed;
            } else {
                return OnStageStatus.Visible;
            }
        }

        public static bool IsNoteIncoming(RuntimeNote note, double now, NoteAnimationMetrics metrics) {
            return GetOnStageStatusOf(note, now, metrics) == OnStageStatus.Incoming;
        }

        public static bool IsNoteIncoming(RuntimeNote note, double now, NoteTimePoints timePoints) {
            return GetOnStageStatusOf(note, now, timePoints) == OnStageStatus.Incoming;
        }

        public static bool IsNoteVisible(RuntimeNote note, double now, NoteAnimationMetrics metrics) {
            return GetOnStageStatusOf(note, now, metrics) == OnStageStatus.Visible;
        }

        public static bool IsNoteVisible(RuntimeNote note, double now, NoteTimePoints timePoints) {
            return GetOnStageStatusOf(note, now, timePoints) == OnStageStatus.Visible;
        }

        public static bool IsNotePassed(RuntimeNote note, double now, NoteAnimationMetrics metrics) {
            return GetOnStageStatusOf(note, now, metrics) == OnStageStatus.Passed;
        }

        public static bool IsNotePassed(RuntimeNote note, double now, NoteTimePoints timePoints) {
            return GetOnStageStatusOf(note, now, timePoints) == OnStageStatus.Passed;
        }

    }
}
