using OpenMLTD.MilliSim.Core.Entities.Runtime;

namespace OpenMLTD.MilliSim.Theater.Intenal {
    internal static class NoteAnimationHelper {

        internal static NoteTimePoints CalculateNoteTimePoints(RuntimeNote note, NoteMetrics metrics) {
            // Empirical formula: s = pow(game_setting, 3) * pow(note_speed, 2)
            var speedScale = metrics.GlobalSpeedScale;
            var relativeSpeed = note.RelativeSpeed;
            var absoluteSpeed = speedScale * speedScale * speedScale * relativeSpeed * relativeSpeed;
            var leadTimeScaled = (float)note.LeadTime / absoluteSpeed;
            return new NoteTimePoints(note.HitTime - leadTimeScaled, note.HitTime);
        }

        internal static OnStageStatus GetOnStageStatusOf(RuntimeNote note, double now, NoteMetrics metrics) {
            var timePoints = CalculateNoteTimePoints(note, metrics);
            return GetOnStageStatusOf(note, now, timePoints);
        }

        internal static OnStageStatus GetOnStageStatusOf(RuntimeNote note, double now, NoteTimePoints timePoints) {
            if (now < timePoints.Enter) {
                return OnStageStatus.Incoming;
            } else if (now > timePoints.Leave) {
                return OnStageStatus.Passed;
            } else {
                return OnStageStatus.Visible;
            }
        }

        internal static bool IsNoteVisible(RuntimeNote note, double now, NoteMetrics metrics) {
            return GetOnStageStatusOf(note, now, metrics) == OnStageStatus.Visible;
        }

        internal static bool IsNoteVisible(RuntimeNote note, double now, NoteTimePoints timePoints) {
            return GetOnStageStatusOf(note, now, timePoints) == OnStageStatus.Visible;
        }

    }
}
