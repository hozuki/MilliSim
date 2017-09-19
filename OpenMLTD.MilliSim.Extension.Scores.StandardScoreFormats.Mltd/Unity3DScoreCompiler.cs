using System;
using System.Collections.Generic;
using System.Linq;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Core.Entities.Extending;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Source;

namespace OpenMLTD.MilliSim.Extension.Scores.StandardScoreFormats.Mltd {
    internal sealed class Unity3DScoreCompiler : DisposableBase, IScoreCompiler {

        /// <summary>
        /// Compiles a <see cref="SourceScore"/> to a <see cref="RuntimeScore"/>, which will be used by the player.
        /// A <see cref="ScoreCompileOptions"/> object can be specified.
        /// </summary>
        /// <param name="score">The <see cref="SourceScore"/> to compile.</param>
        /// <param name="options">Compile options.</param>
        /// <returns>Compiled score.</returns>
        public RuntimeScore Compile(SourceScore score, ScoreCompileOptions options) {
            if (score == null) {
                throw new ArgumentNullException(nameof(score));
            }
            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }

            // ReSharper cannot infer from "(score.Notes?.Count ?? 0) == 0" that score.Notes is later no longer null.
            if (score.Notes.Length == 0) {
                var notes = new RuntimeNote[0];
                return new RuntimeScore(notes) {
                    OffsetToMusic = options.GetValue<float>(ScoreCompileOptions.GlobalSpeedKey)
                };
            }

            var gameNotes = score.Notes;

            var list = new List<RuntimeNote>();

            var conductors = score.Conductors;
            var currentID = 0;
            foreach (var note in gameNotes) {
                RuntimeNote[] notesToBeAdded;

                switch (note.Type) {
                    case NoteType.Tap:
                        notesToBeAdded = CreateTap(note, conductors, ref currentID);
                        break;
                    case NoteType.Flick:
                        notesToBeAdded = CreateFlick(note, conductors, ref currentID);
                        break;
                    case NoteType.Hold:
                        notesToBeAdded = CreateHoldOrSlide(note, conductors, ref currentID);
                        break;
                    case NoteType.Slide:
                        notesToBeAdded = CreateHoldOrSlide(note, conductors, ref currentID);
                        break;
                    case NoteType.Special:
                        notesToBeAdded = CreateSpecial(note, conductors, gameNotes, ref currentID);
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
                OffsetToMusic = score.MusicOffset,
                TrackCount = score.TrackCount
            };
        }

        protected override void Dispose(bool disposing) {
        }

        private static RuntimeNote[] CreateTap(SourceNote note, Conductor[] conductors, ref int currentID) {
            var rn = new RuntimeNote();

            rn.Type = note.Type;
            rn.Size = note.Size;
            rn.FlickDirection = note.FlickDirection;

            rn.ID = ++currentID;
            rn.HitTime = TicksToSeconds(note.Ticks, conductors);
            rn.LeadTime = note.LeadTime;
            rn.RelativeSpeed = note.Speed;
            rn.StartX = note.StartX;
            rn.EndX = note.EndX;

            return new[] { rn };
        }

        private static RuntimeNote[] CreateFlick(SourceNote note, Conductor[] conductors, ref int currentID) {
            var rn = new RuntimeNote();

            rn.Type = note.Type;
            rn.Size = note.Size;
            rn.FlickDirection = note.FlickDirection;

            rn.ID = ++currentID;
            rn.HitTime = TicksToSeconds(note.Ticks, conductors);
            rn.LeadTime = note.LeadTime;
            rn.RelativeSpeed = note.Speed;
            rn.Type = NoteType.Flick;
            rn.StartX = note.StartX;
            rn.EndX = note.EndX;

            // Space reserved for GroupID which may be used in the future.

            return new[] { rn };
        }

        private static RuntimeNote[] CreateHoldOrSlide(SourceNote note, Conductor[] conductors, ref int currentID) {
            // The first polypoint is always the slide start, indicating the end position.
            if (note.PolyPoints == null) {
                throw new ArgumentException("A hold or slide note must have polypoints.", nameof(note));
            }
            if (note.PolyPoints.Length < 2) {
                throw new ArgumentException("A hold or slide note must have at least 2 polypoints.", nameof(note));
            }

            var noteType = note.Type;

            var rn = new RuntimeNote();

            rn.ID = ++currentID;
            rn.HitTime = TicksToSeconds(note.Ticks, conductors);
            rn.LeadTime = note.LeadTime;
            rn.RelativeSpeed = note.Speed;
            rn.Type = noteType;
            rn.StartX = note.StartX;
            rn.EndX = note.EndX;

            var ret = new RuntimeNote[note.PolyPoints.Length];
            ret[0] = rn;

            for (var i = 1; i < note.PolyPoints.Length; ++i) {
                var polyPoint = note.PolyPoints[i];
                var n = new RuntimeNote();
                n.ID = ++currentID;
                n.HitTime = TicksToSeconds(note.Ticks + polyPoint.Subtick, conductors);
                n.LeadTime = rn.LeadTime;
                n.RelativeSpeed = rn.RelativeSpeed;
                n.Type = noteType;
                n.StartX = note.PolyPoints[i - 1].PositionX;
                n.EndX = polyPoint.PositionX;
                ret[i] = n;
            }

            switch (noteType) {
                case NoteType.Hold:
                    for (var i = 0; i < ret.Length - 1; ++i) {
                        ret[i].NextHold = ret[i + 1];
                        ret[i + 1].PrevHold = ret[i];
                    }
                    break;
                case NoteType.Slide:
                    for (var i = 0; i < ret.Length - 1; ++i) {
                        ret[i].NextSlide = ret[i + 1];
                        ret[i + 1].PrevSlide = ret[i];
                    }
                    break;
            }

            ret[ret.Length - 1].FlickDirection = note.FlickDirection;

            return ret;
        }

        private static RuntimeNote[] CreateSpecial(SourceNote note, Conductor[] conductors, SourceNote[] gamingNotes, ref int currentID) {
            var rn = new RuntimeNote();
            rn.ID = ++currentID;
            rn.HitTime = TicksToSeconds(note.Ticks, conductors);
            rn.LeadTime = note.LeadTime;
            rn.RelativeSpeed = note.Speed;
            rn.Type = NoteType.Special;
            rn.StartX = note.StartX;
            rn.EndX = note.EndX;

            var prepare = new RuntimeNote();
            prepare.ID = ++currentID;
            // 0.8 seconds before the Special note.
            // Just a guess.
            // This value must keep the same as tap points' "transform" animation length.
            // See NoteReactor.Update() for more information.
            prepare.HitTime = rn.HitTime - 0.8;
            prepare.LeadTime = rn.LeadTime;
            prepare.RelativeSpeed = rn.RelativeSpeed;
            prepare.Type = NoteType.SpecialPrepare;
            prepare.StartX = rn.StartX;
            prepare.EndX = rn.EndX;

            var end = new RuntimeNote();
            end.ID = ++currentID;
            // 1.5 seconds before next valid note (tap, flick, hold, slide).
            // Just a guess. Didn't find any proof or ways to calculate this.
            // This value must keep the same as tap points' "fade in" animation length.
            // See NoteReactor.Update() for more information.
            var firstNoteAfterSpecial = gamingNotes.FirstOrDefault(n => n.Ticks > note.Ticks);
            if (firstNoteAfterSpecial == null) {
                throw new ArgumentException("Malformed score: no note after special note.");
            }

            end.HitTime = TicksToSeconds(firstNoteAfterSpecial.Ticks, conductors) - 1.5;
            end.LeadTime = rn.LeadTime;
            end.RelativeSpeed = rn.RelativeSpeed;
            end.Type = NoteType.SpecialEnd;
            end.StartX = rn.StartX;
            end.EndX = rn.EndX;

            // The order here is not very important because later they will all be sorted by HitTime.
            return new[] { rn, prepare, end };
        }

        /// <summary>
        /// Gets the absolute time of a given tick, according to a list of <see cref="Conductor"/> information.
        /// </summary>
        /// <param name="currentTicks">A note's literal tick value.</param>
        /// <param name="conductors">Conductor list.</param>
        /// <returns>Seconds.</returns>
        private static double TicksToSeconds(long currentTicks, Conductor[] conductors) {
            var index = Array.FindLastIndex(conductors, conductor => conductor.Ticks <= currentTicks);

            if (index < 0) {
                throw new IndexOutOfRangeException();
            }

            double absoluteTime = 0;
            for (var i = 0; i <= index; ++i) {
                var tempo = conductors[i].Tempo;
                var thisTicks = i < index ? conductors[i + 1].Ticks : currentTicks;
                var deltaTicks = thisTicks - conductors[i].Ticks;
                var deltaTime = deltaTicks / (tempo * NoteBase.TicksPerBeat);
                absoluteTime += deltaTime;
            }

            return absoluteTime;
        }

    }
}
