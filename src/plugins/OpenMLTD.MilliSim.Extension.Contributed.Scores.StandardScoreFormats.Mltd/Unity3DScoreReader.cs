using System;
using System.IO;
using System.Linq;
using OpenMLTD.MilliSim.Contributed.Scores;
using OpenMLTD.MilliSim.Contributed.Scores.Extending;
using OpenMLTD.MilliSim.Contributed.Scores.Runtime;
using OpenMLTD.MilliSim.Contributed.Scores.Source;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.Mltd.Internal;
using OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.Mltd.Serialization;
using UnityStudio.Extensions;
using UnityStudio.Models;
using UnityStudio.Serialization;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.Mltd {
    internal sealed class Unity3DScoreReader : DisposableBase, IScoreReader {

        public bool IsStreamingSupported => true;

        public SourceScore ReadSourceScore(Stream stream, string fileName, ReadSourceOptions sourceOptions) {
            var extension = Path.GetExtension(fileName);
            if (extension == null) {
                throw new ArgumentException();
            }

            var lz4Extension = Unity3DScoreFormat.Unity3DReadExtensions[1];
            var isCompressed = extension.ToLowerInvariant().EndsWith(lz4Extension);
            ScoreObject scoreObject = null;

            using (var bundle = new BundleFile(stream, fileName, isCompressed)) {
                foreach (var asset in bundle.AssetFiles) {
                    foreach (var preloadData in asset.PreloadDataList) {
                        if (preloadData.KnownType != KnownClassID.MonoBehaviour) {
                            continue;
                        }

                        var behaviour = preloadData.LoadAsMonoBehaviour(true);
                        if (!behaviour.Name.Contains("fumen")) {
                            continue;
                        }

                        behaviour = preloadData.LoadAsMonoBehaviour(false);
                        var serializer = new MonoBehaviourSerializer();
                        scoreObject = serializer.Deserialize<ScoreObject>(behaviour);
                        break;
                    }
                }
            }

            if (scoreObject == null) {
                throw new FormatException();
            }

            return ToSourceScore(scoreObject, sourceOptions);
        }

        public RuntimeScore ReadCompiledScore(Stream stream, string fileName, ReadSourceOptions sourceOptions, ScoreCompileOptions compileOptions) {
            var score = ReadSourceScore(stream, fileName, sourceOptions);
            using (var compiler = new Unity3DScoreCompiler()) {
                return compiler.Compile(score, compileOptions);
            }
        }

        protected override void Dispose(bool disposing) {
        }

        private static SourceScore ToSourceScore(ScoreObject scoreObject, ReadSourceOptions options) {
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

                        var n = new SourceNote();
                        var followingNotes = new[] { n };
                        n.StartX = note.StartX;
                        n.EndX = note.EndX;
                        // MLTD tick is divided by 8.
                        n.Ticks = note.Ticks + noteData.Duration * (NoteBase.TicksPerBeat / 8);
                        n.Speed = noteData.Speed;
                        n.LeadTime = noteData.LeadTime;
                        n.Type = note.Type;

                        switch ((MltdNoteEndType)noteData.EndType) {
                            case MltdNoteEndType.Tap:
                                break;
                            case MltdNoteEndType.FlickLeft:
                                n.FlickDirection = FlickDirection.Left;
                                break;
                            case MltdNoteEndType.FlickUp:
                                n.FlickDirection = FlickDirection.Up;
                                break;
                            case MltdNoteEndType.FlickRight:
                                n.FlickDirection = FlickDirection.Right;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        note.FollowingNotes = followingNotes;
                    }
                    break;
                case MltdNoteType.SlideSmall: {
                        note.Type = NoteType.Slide;
                        note.Size = NoteSize.Small;

                        var followingNotes = new SourceNote[noteData.Polypoints.Length - 1];

                        for (var i = 1; i < noteData.Polypoints.Length; ++i) {
                            var poly = noteData.Polypoints[i];
                            var followingNote = new SourceNote {
                                StartX = noteData.Polypoints[i - 1].PositionX,
                                EndX = poly.PositionX,
                                // MLTD tick is divided by 8.
                                Ticks = note.Ticks + poly.SubTick * (NoteBase.TicksPerBeat / 8),
                                Speed = noteData.Speed,
                                LeadTime = noteData.LeadTime,
                                Type = note.Type
                            };

                            if (i == noteData.Polypoints.Length - 1) {
                                switch ((MltdNoteEndType)noteData.EndType) {
                                    case MltdNoteEndType.Tap:
                                        break;
                                    case MltdNoteEndType.FlickLeft:
                                        followingNote.FlickDirection = FlickDirection.Left;
                                        break;
                                    case MltdNoteEndType.FlickUp:
                                        followingNote.FlickDirection = FlickDirection.Up;
                                        break;
                                    case MltdNoteEndType.FlickRight:
                                        followingNote.FlickDirection = FlickDirection.Right;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }

                            followingNotes[i - 1] = followingNote;
                        }

                        note.FollowingNotes = followingNotes;
                    }
                    break;
                case MltdNoteType.Special:
                    note.Type = NoteType.Special;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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

        private static NoteType MapMltdNoteType(MltdNoteType mltdNoteType) {
            switch (mltdNoteType) {
                case MltdNoteType.TapSmall:
                case MltdNoteType.TapLarge:
                    return NoteType.Tap;
                case MltdNoteType.FlickLeft:
                case MltdNoteType.FlickUp:
                case MltdNoteType.FlickRight:
                    return NoteType.Flick;
                case MltdNoteType.HoldSmall:
                case MltdNoteType.HoldLarge:
                    return NoteType.Hold;
                case MltdNoteType.SlideSmall:
                    return NoteType.Slide;
                case MltdNoteType.Special:
                    return NoteType.Special;
                case MltdNoteType.PrimaryBeat:
                case MltdNoteType.SecondaryBeat:
                    return NoteType.Invalid;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mltdNoteType), mltdNoteType, null);
            }
        }

    }
}
