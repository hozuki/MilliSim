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
        public RuntimeScore Compile(Score score, IFlexibleOptions options) {
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

            var conductors = score.Conductors;
            var currentID = 0;
            foreach (var note in gameNotes) {
                RuntimeNote[] notesToBeAdded;

                switch (note.Type) {
                    case NoteType.TapSmall:
                    case NoteType.TapLarge:
                        notesToBeAdded = CreateTap(note, conductors, ref currentID);
                        break;
                    case NoteType.FlickLeft:
                    case NoteType.FlickUp:
                    case NoteType.FlickRight:
                        notesToBeAdded = CreateFlick(note, conductors, ref currentID);
                        break;
                    case NoteType.HoldSmall:
                    case NoteType.HoldLarge:
                        notesToBeAdded = CreateHold(note, conductors, ref currentID);
                        break;
                    case NoteType.SlideSmall:
                        notesToBeAdded = CreateSlide(note, conductors, ref currentID);
                        break;
                    case NoteType.Special:
                        notesToBeAdded = CreateSpecial(note, conductors, ref currentID);
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
                if (note.HitTime.Equals(nextNote.HitTime)) {
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

        private static RuntimeNote[] CreateTap(Note note, Conductor[] conductors, ref int currentID) {
            var rn = new RuntimeNote();

            rn.ID = ++currentID;
            rn.HitTime = note.AbsoluteTime;
            rn.LeadTime = note.LeadTime;
            rn.RelativeSpeed = note.Speed;
            rn.StartX = note.StartPosition;
            rn.EndX = note.EndPosition;

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

        private static RuntimeNote[] CreateFlick(Note note, Conductor[] conductors, ref int currentID) {
            var rn = new RuntimeNote();

            rn.ID = ++currentID;
            rn.HitTime = note.AbsoluteTime;
            rn.LeadTime = note.LeadTime;
            rn.RelativeSpeed = note.Speed;
            rn.Type = RuntimeNoteType.Flick;
            rn.StartX = note.StartPosition;
            rn.EndX = note.EndPosition;

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

        private static RuntimeNote[] CreateHold(Note note, Conductor[] conductors, ref int currentID) {
            var rn = new RuntimeNote();

            rn.ID = ++currentID;
            rn.HitTime = note.AbsoluteTime;
            rn.LeadTime = note.LeadTime;
            rn.RelativeSpeed = note.Speed;
            rn.Type = RuntimeNoteType.Hold;
            rn.StartX = note.StartPosition;
            rn.EndX = note.EndPosition;

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
            holdEnd.HitTime = TicksToSeconds(note.Tick + note.Duration, conductors);
            holdEnd.LeadTime = rn.LeadTime;
            holdEnd.RelativeSpeed = rn.RelativeSpeed;
            holdEnd.Type = rn.Type;
            holdEnd.StartX = rn.EndX;
            holdEnd.EndX = rn.EndX;

            switch (note.EndType) {
                case NoteEndType.Tap:
                    holdEnd.Size = rn.Size;
                    break;
                case NoteEndType.FlickLeft:
                    holdEnd.FlickDirection = FlickDirection.Left;
                    break;
                case NoteEndType.FlickUp:
                    holdEnd.FlickDirection = FlickDirection.Up;
                    break;
                case NoteEndType.FlickRight:
                    holdEnd.FlickDirection = FlickDirection.Right;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            rn.NextHold = holdEnd;
            holdEnd.PrevHold = rn;

            return new[] { rn, holdEnd };
        }

        private static RuntimeNote[] CreateSlide(Note note, Conductor[] conductors, ref int currentID) {
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

            var ret = new RuntimeNote[note.PolyPoints.Length];
            ret[0] = rn;

            for (var i = 1; i < note.PolyPoints.Length; ++i) {
                var polyPoint = note.PolyPoints[i];
                var n = new RuntimeNote();
                n.ID = ++currentID;
                n.HitTime = TicksToSeconds(note.Tick + polyPoint.Subtick, conductors);
                n.LeadTime = rn.LeadTime;
                n.RelativeSpeed = rn.RelativeSpeed;
                n.Type = RuntimeNoteType.Slide;
                n.StartX = ret[i - 1].EndX;
                n.EndX = polyPoint.PositionX;
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

        private static RuntimeNote[] CreateSpecial(Note note, Conductor[] conductors, ref int currentID) {
            var rn = new RuntimeNote();

            rn.ID = ++currentID;
            rn.HitTime = note.AbsoluteTime;
            rn.LeadTime = note.LeadTime;
            rn.RelativeSpeed = note.Speed;
            rn.Type = RuntimeNoteType.Special;
            rn.StartX = note.StartPosition;
            rn.EndX = note.EndPosition;

            var end = new RuntimeNote();

            end.ID = ++currentID;
            // 5 seconds (estimated). Didn't find any proof or ways to calculate this.
            end.HitTime = rn.HitTime + 5;
            end.LeadTime = rn.LeadTime;
            end.RelativeSpeed = rn.RelativeSpeed;
            end.Type = RuntimeNoteType.SpecialEnd;
            end.StartX = rn.StartX;
            end.EndX = rn.EndX;

            return new[] { rn, end };
        }

        /// <summary>
        /// Convert a number of ticks to seconds, given a constant tempo.
        /// </summary>
        /// <param name="deltaTicks">Number of ticks.</param>
        /// <param name="tempo">Current tempo.</param>
        /// <remarks>See remarks of <see cref="RuntimeNote.Ticks"/>.</remarks>
        /// <returns>Seconds.</returns>
        private static double TicksToSeconds(long currentTick, Conductor[] conductors) {
            var index = Array.FindLastIndex(conductors, conductor => conductor.Tick <= currentTick);

            if (index < 0) {
                throw new IndexOutOfRangeException();
            }

            var tempo = conductors[index].Tempo;
            var deltaTicks = currentTick - conductors[index].Tick;
            var deltaTime = deltaTicks / (tempo * 8);
            return conductors[index].AbsoluteTime + deltaTime;
        }

    }
}
