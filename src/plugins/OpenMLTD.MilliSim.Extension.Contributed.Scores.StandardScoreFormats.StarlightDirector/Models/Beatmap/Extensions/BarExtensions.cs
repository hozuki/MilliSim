using System;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Beatmap.Extensions {
    public static class BarExtensions {

        public static Note AddNote([NotNull] this Bar bar, int row, NotePosition column) {
            return AddNote(bar, Guid.NewGuid(), row, column);
        }

        public static Note AddNote([NotNull] this Bar bar, int id, int row, NotePosition column) {
            var guid = StarlightID.GetGuidFromInt32(id);
            return AddNote(bar, guid, row, column);
        }

        [NotNull]
        public static Note AddNote([NotNull] this Bar bar, Guid id, int row, NotePosition column) {
            if (row < 0 || row >= bar.GetNumberOfGrids()) {
                throw new ArgumentOutOfRangeException(nameof(row), row, null);
            }

            if (column == NotePosition.Default) {
                throw new ArgumentOutOfRangeException(nameof(column), column, null);
            }

            if (bar.Notes.Any(n => n.Basic.IndexInGrid == row && n.Basic.FinishPosition == column)) {
                throw new InvalidOperationException($"A note exists at row {row}, column {column}.");
            }

            var note = new Note(bar, id);

            bar.Notes.Add(note);

            bar.Basic.Score.Project.UsedNoteIDs.Add(id);
            // IndexInGrid must be set before calling FixSyncWhenAdded.
            note.Basic.IndexInGrid = row;
            note.Basic.StartPosition = note.Basic.FinishPosition = column;

            note.FixSyncWhenAdded();

            bar.Notes.Sort(Note.TimingThenPositionComparison);

            return note;
        }

        [NotNull]
        public static Note AddSpecialNote([NotNull] this Bar bar, NoteType specialNoteType) {
            var guid = Guid.NewGuid();

            return AddSpecialNote(bar, guid, specialNoteType);
        }

        [NotNull]
        public static Note AddSpecialNote([NotNull] this Bar bar, int id, NoteType specialNoteType) {
            var guid = StarlightID.GetGuidFromInt32(id);

            return AddSpecialNote(bar, guid, specialNoteType);
        }

        [NotNull]
        public static Note AddSpecialNote([NotNull] this Bar bar, Guid id, NoteType specialNoteType) {
            var note = new Note(bar, id);

            note.Params = new NoteExtraParams(note);
            note.SetSpecialType(specialNoteType);

            bar.Notes.Add(note);
            bar.Basic.Score.Project.UsedNoteIDs.Add(id);

            return note;
        }

        public static Note RemoveNote([NotNull] this Bar bar, Guid id) {
            var note = FindNoteByID(bar, id);

            return note == null ? null : RemoveNote(bar, note);
        }

        public static Note RemoveNote([NotNull] this Bar bar, [CanBeNull] Note note) {
            if (note == null) {
                return null;
            }

            if (!bar.Notes.Contains(note)) {
                throw new ArgumentException("Note is not found in bar.");
            }

            // Relations
            note.ResetToTap();

            bar.Notes.Remove(note);
            bar.Basic.Score.Project.UsedNoteIDs.Remove(note.StarlightID);

            return note;
        }

        [DebuggerStepThrough]
        [CanBeNull]
        // ReSharper disable once InconsistentNaming
        public static Note FindNoteByID([NotNull] this Bar bar, Guid id) {
            return bar.Notes.FirstOrDefault(n => n.StarlightID == id);
        }

        [DebuggerStepThrough]
        public static int GetSignature([NotNull] this Bar bar) {
            return bar.Params?.UserDefinedSignature ?? bar.Basic.Score.Project.Settings.Signature;
        }

        [DebuggerStepThrough]
        public static int GetGridPerSignature([NotNull] this Bar bar) {
            return bar.Params?.UserDefinedGridPerSignature ?? bar.Basic.Score.Project.Settings.GridPerSignature;
        }

        [DebuggerStepThrough]
        public static int GetNumberOfGrids([NotNull] this Bar bar) {
            return GetGridPerSignature(bar) * GetSignature(bar);
        }

        [DebuggerStepThrough]
        public static void EditorToggleSelected([NotNull] this Bar bar) {
            bar.Editor.IsSelected = !bar.Editor.IsSelected;
        }

        [DebuggerStepThrough]
        public static void EditorSelect([NotNull] this Bar bar) {
            bar.Editor.IsSelected = true;
        }

        [DebuggerStepThrough]
        public static void EditorUnselect([NotNull] this Bar bar) {
            bar.Editor.IsSelected = false;
        }

        [DebuggerStepThrough]
        [CanBeNull]
        public static Bar GetNextBar([NotNull] this Bar bar) {
            var index = bar.Basic.Index;
            var score = bar.Basic.Score;

            return index == score.Bars.Count - 1 ? null : score.Bars[index + 1];
        }

        [DebuggerStepThrough]
        [CanBeNull]
        public static Bar GetPreviousBar([NotNull] this Bar bar) {
            var index = bar.Basic.Index;
            return index == 0 ? null : bar.Basic.Score.Bars[index - 1];
        }

        // TODO: refactor with ScoreExtensions.CalculateDuration().
        public static void UpdateStartTime([NotNull] this Bar bar) {
            var score = bar.Basic.Score;
            var notes = score.GetAllNotes();
            var allBpmNotes = notes.Where(n => n.Basic.Type == NoteType.VariantBpm).ToArray();

            var currentTiming = score.Project.Settings.StartTimeOffset;
            var currentBpm = score.Project.Settings.BeatPerMinute;
            var currentInterval = BeatmapMathHelper.BpmToInterval(currentBpm);

            foreach (var b in score.Bars) {
                if (b == bar) {
                    break;
                }

                var currentGridPerSignature = b.GetGridPerSignature();
                var numGrids = b.GetNumberOfGrids();

                if (allBpmNotes.Any(n => n.Basic.Bar == b)) {
                    var bpmNotesInThisBar = allBpmNotes.Where(n => n.Basic.Bar == b).ToList();

                    bpmNotesInThisBar.Sort((n1, n2) => n1.Basic.IndexInGrid.CompareTo(n2.Basic.IndexInGrid));

                    var bpmNoteIndex = 0;

                    for (var i = 0; i < numGrids; ++i) {
                        if (bpmNoteIndex < bpmNotesInThisBar.Count) {
                            var bpmNote = bpmNotesInThisBar[bpmNoteIndex];

                            Debug.Assert(bpmNote.Params != null, "bpmNote.Params != null");

                            if (i == bpmNote.Basic.IndexInGrid) {
                                currentBpm = bpmNote.Params.NewBpm;
                                currentInterval = BeatmapMathHelper.BpmToInterval(currentBpm);

                                ++bpmNoteIndex;
                            }
                        }

                        currentTiming += currentInterval / currentGridPerSignature;
                    }
                } else {
                    var currentSignature = b.GetSignature();

                    currentTiming += currentInterval * currentSignature;
                }
            }

            var startTime = TimeSpan.FromSeconds(currentTiming);

            bar.Temporary.StartTime = startTime;
        }

        public static TimeSpan CalculateDuration([NotNull] this Bar bar) {
            var notes = bar.Notes;
            var score = bar.Basic.Score;
            var currentBpm = score.Project.Settings.BeatPerMinute;

            foreach (var b in score.Bars) {
                if (b == bar) {
                    break;
                }

                var thisBarHasBpm = b.Notes.Count != 0 && b.Notes.Any(n => n.Basic.Type == NoteType.VariantBpm);

                if (!thisBarHasBpm) {
                    continue;
                }

                var bpmNotes = b.Notes.Where(n => n.Basic.Type == NoteType.VariantBpm).ToList();

                bpmNotes.Sort(Note.TimingComparison);

                var currentBpmNote = bpmNotes[bpmNotes.Count - 1];

                Debug.Assert(currentBpmNote.Params != null, "currentBpmNote.Params != null");

                currentBpm = currentBpmNote.Params.NewBpm;
            }

            var hasAnyBpmNote = notes.Count != 0 && notes.Any(n => n.Basic.Type == NoteType.VariantBpm);
            var currentInterval = BeatmapMathHelper.BpmToInterval(currentBpm);
            double seconds;

            if (hasAnyBpmNote) {
                var bpmNotesInThisBar = bar.Notes.Where(n => n.Basic.Bar == bar).ToList();
                var numGrids = bar.GetNumberOfGrids();
                var currentGridPerSignature = bar.GetGridPerSignature();

                bpmNotesInThisBar.Sort((n1, n2) => n1.Basic.IndexInGrid.CompareTo(n2.Basic.IndexInGrid));

                var bpmNoteIndex = 0;

                seconds = 0;

                for (var i = 0; i < numGrids; ++i) {
                    if (bpmNoteIndex < bpmNotesInThisBar.Count) {
                        var bpmNote = bpmNotesInThisBar[bpmNoteIndex];

                        if (i == bpmNote.Basic.IndexInGrid) {
                            currentInterval = BeatmapMathHelper.BpmToInterval(currentBpm);
                            ++bpmNoteIndex;
                        }
                    }

                    seconds += currentInterval * i / currentGridPerSignature;
                }
            } else {
                seconds = BeatmapMathHelper.BpmToInterval(currentBpm) * bar.GetSignature();
            }

            return TimeSpan.FromSeconds(seconds);
        }

        [CanBeNull]
        [ContractAnnotation("note:null => null; note:notnull => notnull")]
        internal static Note RemoveSpecialNoteForVariantBpmFix([NotNull] this Bar bar, [CanBeNull] Note note) {
            if (note == null) {
                return null;
            }

            bar.Notes.Remove(note);
            bar.Basic.Score.Project.UsedNoteIDs.Remove(note.StarlightID);

            return note;
        }

    }
}
