using System.Linq;
using OpenMLTD.MilliSim.Core.Entities;

namespace OpenMLTD.MilliSim.Extension.Imports.Unity3D.Extensions {
    internal static class ScoreObjectExtensions {

        internal static Score ToMilliSimScore(this ScoreObject scoreObject) {
            var score = new Score();
            score.Notes = scoreObject.NoteEvents.Select(ToNote).ToArray();
            score.Conductors = scoreObject.ConductorEvents.Select(ToConductor).ToArray();
            score.JudgeRanges = (float[])scoreObject.JudgeRange.Clone();
            score.ScoreSpeeds = (float[])scoreObject.ScoreSpeed.Clone();
            score.MusicOffset = scoreObject.BgmOffset;
            return score;
        }

        private static Note ToNote(EventNoteData noteData) {
            var note = new Note();
            note.AbsoluteTime = noteData.AbsoluteTime;
            note.IsSelected = noteData.Selected;
            note.Tick = noteData.Tick;
            note.Measure = noteData.Measure;
            note.Beat = noteData.Beat;
            note.TrackIndex = noteData.Track;
            note.Type = (NoteType)noteData.Type;
            note.StartPosition = noteData.StartPositionX;
            note.EndPosition = noteData.EndPositionX;
            note.Speed = noteData.Speed;
            note.Duration = noteData.Duration;
            note.PolyPoints = noteData.Polypoints.Select(poly => new PolyPoint {
                PositionX = poly.PositionX,
                Subtick = poly.SubTick
            }).ToArray();
            note.EndType = (NoteEndType)noteData.EndType;
            note.LeadTime = noteData.LeadTime;
            return note;
        }

        private static Conductor ToConductor(EventConductorData conductorData) {
            var conductor = new Conductor();
            conductor.AbsoluteTime = conductorData.AbsoluteTime;
            conductor.IsSelected = conductorData.Selected;
            conductor.Tick = conductorData.Tick;
            conductor.Measure = conductorData.Measure;
            conductor.Beat = conductorData.Beat;
            conductor.TrackIndex = conductorData.Track;
            conductor.Tempo = conductorData.Tempo;
            conductor.SignatureNumerator = conductorData.SignatureNumerator;
            conductor.SignatureDenominator = conductorData.SignatureDenominator;
            conductor.Marker = conductorData.Marker;
            return conductor;
        }

    }
}
