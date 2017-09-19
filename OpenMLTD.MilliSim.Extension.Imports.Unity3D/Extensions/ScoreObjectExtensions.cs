using System;
using System.Linq;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Core.Entities.Source;
using OpenMLTD.MilliSim.Extension.Imports.Unity3D.Mltd;
using OpenMLTD.MilliSim.Extension.Imports.Unity3D.Serialization;

namespace OpenMLTD.MilliSim.Extension.Imports.Unity3D.Extensions {
    internal static class ScoreObjectExtensions {

        internal static SourceScore ToSourceScore(this ScoreObject scoreObject, ReadSourceOptions options) {
            var score = new SourceScore();

            var scoreIndex = options.ScoreIndex;
            var difficulty = (Difficulty)scoreIndex;
            var trackType = ScoreHelper.MapDifficultyToTrackType(difficulty);
            var tracks = ScoreHelper.GetTrackIndicesFromTrackType(trackType);

            score.Notes = scoreObject.NoteEvents
                .Where(nd => Array.IndexOf(tracks, nd.Track) >= 0)
                .Select(n => ToNote(n, tracks))
                .Where(n => n != null).ToArray();
            score.Conductors = scoreObject.ConductorEvents.Select(ToConductor).ToArray();
            score.MusicOffset = scoreObject.BgmOffset;

            score.ScoreIndex = scoreIndex;
            score.TrackCount = tracks.Length;

            return score;
        }

        private static SourceNote ToNote(EventNoteData noteData, int[] tracks) {
            var mltdNoteType = (MltdNoteType)noteData.Type;
            switch (mltdNoteType) {
                case MltdNoteType.PrimaryBeat:
                case MltdNoteType.SecondaryBeat:
                    // We don't generate notes for these types.
                    return null;
            }

            var note = new SourceNote();
            // MLTD tick is divided by 8.
            note.Ticks = noteData.Tick * (NoteBase.TicksPerBeat / 8);
            note.Measure = noteData.Measure - 1;
            note.Beat = noteData.Beat - 1;
            note.StartX = noteData.StartPositionX;
            note.EndX = noteData.EndPositionX;
            note.Speed = noteData.Speed;
            note.LeadTime = noteData.LeadTime;

            note.TrackIndex = noteData.Track - tracks[0];

            switch (mltdNoteType) {
                case MltdNoteType.TapSmall:
                    note.Type = NoteType.Tap;
                    note.Size = NoteSize.Small;
                    break;
                case MltdNoteType.TapLarge:
                    note.Type = NoteType.Tap;
                    note.Size = NoteSize.Large;
                    break;
                case MltdNoteType.FlickLeft:
                    note.Type = NoteType.Flick;
                    note.FlickDirection = FlickDirection.Left;
                    break;
                case MltdNoteType.FlickUp:
                    note.Type = NoteType.Flick;
                    note.FlickDirection = FlickDirection.Up;
                    break;
                case MltdNoteType.FlickRight:
                    note.Type = NoteType.Flick;
                    note.FlickDirection = FlickDirection.Right;
                    break;
                case MltdNoteType.HoldSmall:
                case MltdNoteType.HoldLarge: {
                        note.Type = NoteType.Hold;
                        note.Size = mltdNoteType == MltdNoteType.HoldSmall ? NoteSize.Small : NoteSize.Large;

                        var generatedPolyPoints = new[] { new PolyPoint(), new PolyPoint() };
                        generatedPolyPoints[0].Subtick = 0;
                        generatedPolyPoints[0].PositionX = note.EndX;
                        // MLTD tick is divided by 8.
                        generatedPolyPoints[1].Subtick = noteData.Duration * (NoteBase.TicksPerBeat / 8);
                        generatedPolyPoints[1].PositionX = note.EndX;
                        note.PolyPoints = generatedPolyPoints;
                    }
                    break;
                case MltdNoteType.SlideSmall:
                    note.Type = NoteType.Slide;
                    note.Size = NoteSize.Small;
                    note.PolyPoints = noteData.Polypoints.Select(poly => new PolyPoint {
                        PositionX = poly.PositionX,
                        // MLTD tick is divided by 8.
                        Subtick = poly.SubTick * (NoteBase.TicksPerBeat / 8)
                    }).ToArray();
                    break;
                case MltdNoteType.Special:
                    note.Type = NoteType.Special;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (mltdNoteType) {
                case MltdNoteType.HoldSmall:
                case MltdNoteType.SlideSmall:
                case MltdNoteType.HoldLarge:
                    switch ((MltdNoteEndType)noteData.EndType) {
                        case MltdNoteEndType.Tap:
                            note.FlickDirection = FlickDirection.None;
                            break;
                        case MltdNoteEndType.FlickLeft:
                            note.FlickDirection = FlickDirection.Left;
                            break;
                        case MltdNoteEndType.FlickUp:
                            note.FlickDirection = FlickDirection.Up;
                            break;
                        case MltdNoteEndType.FlickRight:
                            note.FlickDirection = FlickDirection.Right;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
            }

            return note;
        }

        private static Conductor ToConductor(EventConductorData conductorData) {
            var conductor = new Conductor();
            // MLTD tick is divided by 8.
            conductor.Ticks = conductorData.Tick * (NoteBase.TicksPerBeat / 8);
            conductor.Measure = conductorData.Measure - 1;
            conductor.Beat = conductorData.Beat - 1;
            conductor.Tempo = conductorData.Tempo;
            conductor.SignatureNumerator = conductorData.SignatureNumerator;
            conductor.SignatureDenominator = conductorData.SignatureDenominator;
            return conductor;
        }

    }
}
