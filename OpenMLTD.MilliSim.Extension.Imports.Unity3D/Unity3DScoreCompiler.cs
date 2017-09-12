using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Core.Entities.Extending;
using OpenMLTD.MilliSim.Core.Entities.Runtime;

namespace OpenMLTD.MilliSim.Extension.Imports.Unity3D {
    public sealed class Unity3DScoreCompiler : DisposableBase, IScoreCompiler {

        internal Unity3DScoreCompiler() {
        }

        /// <summary>
        /// Compiles a <see cref="Score"/> to a <see cref="RuntimeScore"/>, which will be used by the player.
        /// </summary>
        /// <param name="score">The <see cref="Score"/> to compile.</param>
        /// <returns>Compiled score.</returns>
        public RuntimeScore Compile([NotNull] Score score) {
            return Compile(score, ScoreCompileOptions.Default);
        }

        /// <summary>
        /// Compiles a <see cref="Score"/> to a <see cref="RuntimeScore"/>, which will be used by the player.
        /// A <see cref="ScoreCompileOptions"/> object can be specified.
        /// </summary>
        /// <param name="score">The <see cref="Score"/> to compile.</param>
        /// <param name="options">Compile options.</param>
        /// <returns>Compiled score.</returns>
        public RuntimeScore Compile([NotNull] Score score, [NotNull] IFlexibleOptions options) {
            if (score == null) {
                throw new ArgumentNullException(nameof(score));
            }
            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }

            // ReSharper cannot infer from "(score.Notes?.Count ?? 0) == 0" that score.Notes is later no longer null.
            if (score.Notes == null || score.Notes.Length == 0) {
                var notes = new RuntimeNote[0];
                return new RuntimeScore(notes) {
                    Difficulty = options.GetValue<Difficulty>(ScoreCompileOptions.DifficultyKey),
                    OffsetToMusic = options.GetValue<float>(ScoreCompileOptions.GlobalSpeedKey)
                };
            }

            var trackType = ScoreHelper.MapDifficultyToTrackType(options.GetValue<Difficulty>(ScoreCompileOptions.DifficultyKey));
            var trackIndices = ScoreHelper.GetTrackIndicesFromTrackType(trackType);
            var gameNotes = score.Notes.Where(n => Array.IndexOf(trackIndices, n.TrackIndex) >= 0).ToArray();

            var list = new List<RuntimeNote>();

            var currentID = 0;
            foreach (var note in gameNotes) {
                RuntimeNote[] notesToBeAdded;

                switch (note.Type) {
                    case NoteType.TapSmall:
                    case NoteType.TapLarge:
                        notesToBeAdded = CreateTap(note, ref currentID);
                        break;
                    case NoteType.FlickLeft:
                    case NoteType.FlickUp:
                    case NoteType.FlickRight:
                        notesToBeAdded = CreateFlick(note, ref currentID);
                        break;
                    case NoteType.HoldSmall:
                    case NoteType.HoldLarge:
                        notesToBeAdded = CreateHold(note, ref currentID);
                        break;
                    case NoteType.SlideSmall:
                        notesToBeAdded = CreateSlide(note, ref currentID);
                        break;
                    case NoteType.Special:
                        notesToBeAdded = CreateSpecial(note, ref currentID);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                list.AddRange(notesToBeAdded);
            }

            var totalNotesCount = list.Count;
            list.Sort(RuntimeNoteComparisons.ByTimeThenX);

            // Fix sync relations. Notice that you should keep NextSync's accessing order being after PrevSync.
            for (var i = 0; i < totalNotesCount - 1; ++i) {
                var note = list[i];
                var nextNote = list[i + 1];
                // About this strange behavior, see remarks on RuntimeNote.Ticks.
                if (note.HitTime.Equals(nextNote.HitTime) || note.Ticks == nextNote.Ticks) {
                    note.NextSync = nextNote;
                    nextNote.PrevSync = note;
                }
            }

            // TODO: Fix flick/slide group.

            var runtimeNotes = list.ToArray();
            return new RuntimeScore(runtimeNotes) {
                Difficulty = options.GetValue<Difficulty>(ScoreCompileOptions.DifficultyKey),
                OffsetToMusic = score.MusicOffset
            };
        }

        protected override void Dispose(bool disposing) {
        }

        private static RuntimeNote[] CreateTap(Note note, ref int currentID) {
            var rn = new RuntimeNote();

            rn.ID = ++currentID;
            rn.HitTime = note.AbsoluteTime;
            rn.LeadTime = note.LeadTime;
            rn.RelativeSpeed = note.Speed;
            rn.StartX = note.StartPosition;
            rn.EndX = note.EndPosition;
            rn.Ticks = note.Tick;

            switch (note.Type) {
                case NoteType.TapSmall:
                    rn.Type = RuntimeNoteType.Tap;
                    rn.Size = RuntimeNoteSize.Small;
                    break;
                case NoteType.TapLarge:
                    rn.Type = RuntimeNoteType.Tap;
                    rn.Size = RuntimeNoteSize.Large;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new[] { rn };
        }

        private static RuntimeNote[] CreateFlick(Note note, ref int currentID) {
            var rn = new RuntimeNote();

            rn.ID = ++currentID;
            rn.HitTime = note.AbsoluteTime;
            rn.LeadTime = note.LeadTime;
            rn.RelativeSpeed = note.Speed;
            rn.Type = RuntimeNoteType.Flick;
            rn.StartX = note.StartPosition;
            rn.EndX = note.EndPosition;
            rn.Ticks = note.Tick;

            switch (note.Type) {
                case NoteType.FlickLeft:
                    rn.FlickDirection = FlickDirection.Left;
                    break;
                case NoteType.FlickUp:
                    rn.FlickDirection = FlickDirection.Up;
                    break;
                case NoteType.FlickRight:
                    rn.FlickDirection = FlickDirection.Right;
                    break;
            }

            // Space reserved for GroupID which may be used in the future.

            return new[] { rn };
        }

        private static RuntimeNote[] CreateHold(Note note, ref int currentID) {
            var rn = new RuntimeNote();

            rn.ID = ++currentID;
            rn.HitTime = note.AbsoluteTime;
            rn.LeadTime = note.LeadTime;
            rn.RelativeSpeed = note.Speed;
            rn.Type = RuntimeNoteType.Hold;
            rn.StartX = note.StartPosition;
            rn.EndX = note.EndPosition;
            rn.Ticks = note.Tick;

            switch (note.Type) {
                case NoteType.HoldSmall:
                    rn.Size = RuntimeNoteSize.Small;
                    break;
                case NoteType.HoldLarge:
                    rn.Size = RuntimeNoteSize.Large;
                    break;
            }

            // Creates a HoldEnd note.
            var holdEnd = new RuntimeNote();
            holdEnd.ID = ++currentID;
            holdEnd.HitTime = rn.HitTime + RuntimeNoteHelper.TicksToSeconds(note.Duration);
            holdEnd.LeadTime = rn.LeadTime;
            holdEnd.RelativeSpeed = rn.RelativeSpeed;
            holdEnd.StartX = rn.EndX;
            holdEnd.EndX = rn.EndX;
            holdEnd.Ticks = rn.Ticks + note.Duration;

            switch (note.EndType) {
                case NoteEndType.Tap:
                    holdEnd.Type = RuntimeNoteType.Tap;
                    holdEnd.Size = rn.Size;
                    break;
                case NoteEndType.FlickLeft:
                    holdEnd.Type = RuntimeNoteType.Flick;
                    holdEnd.FlickDirection = FlickDirection.Left;
                    break;
                case NoteEndType.FlickUp:
                    holdEnd.Type = RuntimeNoteType.Flick;
                    holdEnd.FlickDirection = FlickDirection.Up;
                    break;
                case NoteEndType.FlickRight:
                    holdEnd.Type = RuntimeNoteType.Flick;
                    holdEnd.FlickDirection = FlickDirection.Right;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            rn.NextHold = holdEnd;
            holdEnd.PrevHold = rn;

            return new[] { rn, holdEnd };
        }

        private static RuntimeNote[] CreateSlide(Note note, ref int currentID) {
            // The first polypoint is always the slide start, indicating the end position.
            if (note.PolyPoints == null) {
                throw new ArgumentException("A slide note must have polypoints.", nameof(note));
            }
            if (note.PolyPoints.Length < 2) {
                throw new ArgumentException("A slide note must have at least 2 polypoints.", nameof(note));
            }

            var rn = new RuntimeNote();

            rn.ID = ++currentID;
            rn.HitTime = note.AbsoluteTime;
            rn.LeadTime = note.LeadTime;
            rn.RelativeSpeed = note.Speed;
            rn.Type = RuntimeNoteType.Slide;
            rn.StartX = note.StartPosition;
            rn.EndX = note.EndPosition;
            rn.Ticks = note.Tick;

            var ret = new RuntimeNote[note.PolyPoints.Length];
            ret[0] = rn;

            for (var i = 1; i < note.PolyPoints.Length; ++i) {
                var polyPoint = note.PolyPoints[i];
                var n = new RuntimeNote();
                n.ID = ++currentID;
                n.HitTime = rn.HitTime + RuntimeNoteHelper.TicksToSeconds(polyPoint.Subtick);
                n.LeadTime = rn.LeadTime;
                n.RelativeSpeed = rn.RelativeSpeed;
                n.Type = RuntimeNoteType.Slide;
                n.StartX = ret[i - 1].EndX;
                n.EndX = polyPoint.PositionX;
                n.Ticks = rn.Ticks + polyPoint.Subtick;
                ret[i] = n;
            }

            for (var i = 0; i < ret.Length - 1; ++i) {
                ret[i].NextSlide = ret[i + 1];
                ret[i + 1].PrevSlide = ret[i];
            }

            switch (note.EndType) {
                case NoteEndType.Tap:
                    break;
                case NoteEndType.FlickLeft:
                    ret[ret.Length - 1].FlickDirection = FlickDirection.Left;
                    break;
                case NoteEndType.FlickUp:
                    ret[ret.Length - 1].FlickDirection = FlickDirection.Up;
                    break;
                case NoteEndType.FlickRight:
                    ret[ret.Length - 1].FlickDirection = FlickDirection.Right;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return ret;
        }

        private static RuntimeNote[] CreateSpecial(Note note, ref int currentID) {
            var rn = new RuntimeNote();

            rn.ID = ++currentID;
            rn.HitTime = note.AbsoluteTime;
            rn.LeadTime = note.LeadTime;
            rn.RelativeSpeed = note.Speed;
            rn.Type = RuntimeNoteType.Special;
            rn.StartX = note.StartPosition;
            rn.EndX = note.EndPosition;

            return new[] { rn };
        }

    }
}
