using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Beatmap.Extensions {
    public static class ScoreExtensions {

        [DebuggerStepThrough]
        [NotNull]
        public static CompiledScore Compile([NotNull] this Score score) {
            return Compile(score, null);
        }

        [DebuggerStepThrough]
        [NotNull]
        public static CompiledScore Compile([NotNull] this Score score, TimeSpan userDefinedEnding) {
            return Compile(score, (TimeSpan?)userDefinedEnding);
        }

        [DebuggerStepThrough]
        [NotNull]
        public static Bar AppendBar([NotNull] this Score score) {
            return AppendBar(score, Guid.NewGuid());
        }

        [DebuggerStepThrough]
        [NotNull]
        public static Bar AppendBar([NotNull] this Score score, Guid id) {
            var bar = new Bar(score, score.Bars.Count, id);

            score.Bars.Add(bar);

            return bar;
        }

        [DebuggerStepThrough]
        [NotNull]
        public static Bar AppendBar([NotNull] this Score score, int id) {
            var guid = StarlightID.GetGuidFromInt32(id);

            return AppendBar(score, guid);
        }

        [NotNull, ItemNotNull]
        public static IReadOnlyList<Bar> AppendBars([NotNull] this Score score, int count) {
            if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count), count, "Number of bars must be no less than 0.");
            }

            if (count == 0) {
                return new Bar[0];
            }

            var bars = new Bar[count];
            var firstIndex = score.Bars.Count;

            for (var i = 0; i < bars.Length; ++i) {
                bars[i] = new Bar(score, firstIndex + i);
            }

            score.Bars.AddRange(bars);

            return bars;
        }

        [NotNull]
        public static Bar InsertBar([NotNull] this Score score, int beforeIndex) {
            CheckBarIndex(score, beforeIndex);

            var bar = score.Bars[beforeIndex];

            return InsertBar(score, bar);
        }

        [NotNull]
        public static Bar InsertBar([NotNull] this Score score, int beforeIndex, int id) {
            CheckBarIndex(score, beforeIndex);

            var bar = score.Bars[beforeIndex];

            return InsertBar(score, bar, id);
        }

        [NotNull]
        public static Bar InsertBar([NotNull] this Score score, int beforeIndex, Guid id) {
            CheckBarIndex(score, beforeIndex);

            var bar = score.Bars[beforeIndex];

            return InsertBar(score, bar, id);
        }

        [NotNull]
        public static Bar InsertBar([NotNull] this Score score, [NotNull] Bar beforeBar) {
            var id = Guid.NewGuid();

            return InsertBar(score, beforeBar, id);
        }

        [NotNull]
        public static Bar InsertBar(this Score score, [NotNull] Bar beforeBar, int id) {
            var guid = StarlightID.GetGuidFromInt32(id);

            return InsertBar(score, beforeBar, guid);
        }

        [NotNull]
        public static Bar InsertBar([NotNull] this Score score, [NotNull] Bar beforeBar, Guid id) {
            if (beforeBar == null) {
                throw new ArgumentNullException(nameof(beforeBar));
            }

            var newIndex = beforeBar.Basic.Index;

            foreach (var b in score.Bars) {
                if (b.Basic.Index >= newIndex) {
                    ++b.Basic.Index;
                }
            }

            var bar = new Bar(score, newIndex, id);

            score.Bars.Insert(newIndex, bar);

            return bar;
        }

        [NotNull, ItemNotNull]
        public static IReadOnlyList<Bar> InsertBars([NotNull] this Score score, int beforeIndex, int count) {
            CheckBarIndex(score, beforeIndex);

            var bar = score.Bars[beforeIndex];

            return InsertBars(score, bar, count);
        }

        [NotNull, ItemNotNull]
        public static IReadOnlyList<Bar> InsertBars([NotNull] this Score score, [NotNull] Bar beforeBar, int count) {
            if (beforeBar == null) {
                throw new ArgumentNullException(nameof(beforeBar));
            }

            if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count), count, "Number of bars must be no less than 0.");
            }

            if (count == 0) {
                return new Bar[0];
            }

            var bars = new Bar[count];
            var newIndex = beforeBar.Basic.Index;

            for (var i = 0; i < bars.Length; ++i) {
                bars[i] = new Bar(score, newIndex + i);
            }

            foreach (var b in score.Bars) {
                if (b.Basic.Index >= newIndex) {
                    b.Basic.Index += count;
                }
            }

            score.Bars.InsertRange(newIndex, bars);

            return bars;
        }

        [DebuggerStepThrough]
        [NotNull]
        public static Bar RemoveBar([NotNull] this Score score, [NotNull] Bar bar) {
            if (!score.Bars.Contains(bar)) {
                throw new ArgumentException("Assigned bar is not in the score.", nameof(bar));
            }

            var barIndex = bar.Basic.Index;

            foreach (var b in score.Bars) {
                if (b.Basic.Index > barIndex) {
                    --b.Basic.Index;
                }
            }

            foreach (var note in bar.Notes) {
                note.ResetToTap();
            }

            score.Bars.Remove(bar);

            return bar;
        }

        [DebuggerStepThrough]
        public static void RemoveBars([NotNull] this Score score, [NotNull, ItemNotNull] IReadOnlyList<Bar> bars) {
            foreach (var bar in bars) {
                if (!score.Bars.Contains(bar)) {
                    throw new ArgumentException("Assigned bar is not in the score.", nameof(bar));
                }
            }

            var barIndices = bars.Select(b => b.Basic.Index).ToArray();

            foreach (var bar in bars) {
                foreach (var note in bar.Notes) {
                    note.ResetToTap();
                }

                score.Bars.Remove(bar);
            }

            foreach (var bar in score.Bars) {
                var greaterNum = barIndices.Count(index => bar.Basic.Index > index);
                bar.Basic.Index -= greaterNum;
            }
        }

        [DebuggerStepThrough]
        [CanBeNull]
        // ReSharper disable once InconsistentNaming
        public static Note FindNoteByID([NotNull] this Score score, Guid id) {
            return score.Bars.SelectMany(bar => bar.Notes).FirstOrDefault(note => note.StarlightID == id);
        }

        public static TimeSpan CalculateDuration([NotNull] this Score score) {
            var notes = score.GetAllNotes();
            var allBpmNotes = notes.Where(n => n.Basic.Type == NoteType.VariantBpm).ToArray();

            var currentTiming = score.Project.Settings.StartTimeOffset;
            var currentBpm = score.Project.Settings.BeatPerMinute;
            var currentInterval = BeatmapMathHelper.BpmToInterval(currentBpm);

            foreach (var bar in score.Bars) {
                var currentGridPerSignature = bar.GetGridPerSignature();
                var numGrids = bar.GetNumberOfGrids();

                if (allBpmNotes.Any(n => n.Basic.Bar == bar)) {
                    var bpmNotesInThisBar = allBpmNotes.Where(n => n.Basic.Bar == bar).ToList();

                    bpmNotesInThisBar.Sort((n1, n2) => n1.Basic.IndexInGrid.CompareTo(n2.Basic.IndexInGrid));

                    var bpmNoteIndex = 0;

                    for (var i = 0; i < numGrids; ++i) {
                        if (bpmNoteIndex < bpmNotesInThisBar.Count) {
                            var bpmNote = bpmNotesInThisBar[bpmNoteIndex];

                            if (i == bpmNote.Basic.IndexInGrid) {
                                Debug.Assert(bpmNote.Params != null, "bpmNote.Params != null");

                                currentBpm = bpmNote.Params.NewBpm;
                                currentInterval = BeatmapMathHelper.BpmToInterval(currentBpm);

                                ++bpmNoteIndex;
                            }
                        }

                        currentTiming += currentInterval / currentGridPerSignature;
                    }
                } else {
                    var currentSignature = bar.GetSignature();

                    currentTiming += currentInterval * currentSignature;
                }
            }

            return TimeSpan.FromSeconds(currentTiming);
        }

        public static void UpdateAllStartTimes([NotNull] this Score score) {
            var notes = score.GetAllNotes();
            var allBpmNotes = notes.Where(n => n.Basic.Type == NoteType.VariantBpm).ToArray();

            var currentTiming = score.Project.Settings.StartTimeOffset;
            var currentBpm = score.Project.Settings.BeatPerMinute;
            var currentInterval = BeatmapMathHelper.BpmToInterval(currentBpm);
            var currentBarIndex = 0;

            foreach (var bar in score.Bars) {
                bar.Temporary.StartTime = TimeSpan.FromSeconds(currentTiming);

                if (currentBarIndex == score.Bars.Count - 1) {
                    break;
                }

                var currentGridPerSignature = bar.GetGridPerSignature();
                var numGrids = bar.GetNumberOfGrids();

                if (allBpmNotes.Any(n => n.Basic.Bar == bar)) {
                    var bpmNotesInThisBar = allBpmNotes.Where(n => n.Basic.Bar == bar).ToList();

                    bpmNotesInThisBar.Sort((n1, n2) => n1.Basic.IndexInGrid.CompareTo(n2.Basic.IndexInGrid));

                    var bpmNoteIndex = 0;

                    for (var i = 0; i < numGrids; ++i) {
                        if (bpmNoteIndex < bpmNotesInThisBar.Count) {
                            var bpmNote = bpmNotesInThisBar[bpmNoteIndex];

                            if (i == bpmNote.Basic.IndexInGrid) {
                                Debug.Assert(bpmNote.Params != null, "bpmNote.Params != null");

                                currentBpm = bpmNote.Params.NewBpm;
                                currentInterval = BeatmapMathHelper.BpmToInterval(currentBpm);

                                ++bpmNoteIndex;
                            }
                        }

                        currentTiming += currentInterval / currentGridPerSignature;
                    }
                } else {
                    var currentSignature = bar.GetSignature();

                    currentTiming += currentInterval * currentSignature;
                }

                ++currentBarIndex;
            }
        }

        /// <summary>
        /// Calculate timing information of each note, store the information in <see cref="Note.TemporaryProperties.HitTiming"/>,
        /// and return a detailed timing list for each grid line.
        /// </summary>
        /// <param name="score">The <see cref="Score"/> whose timing information is to be calculated.</param>
        /// <returns>A timing list. The outer index is bar index, and the inner index is grid index.</returns>
        [NotNull]
        public static IReadOnlyList<IReadOnlyList<TimeSpan>> UpdateNoteHitTimings([NotNull] this Score score) {
            var notes = score.GetAllNotes();

            // First, calculate all timing at the grid lines.
            var allBpmNotes = notes.Where(n => n.Basic.Type == NoteType.VariantBpm).ToArray();
            var timings = new List<TimeSpan[]>();


            var currentTiming = TimeSpan.FromSeconds(score.Project.Settings.StartTimeOffset);
            var currentBpm = score.Project.Settings.BeatPerMinute;
            var currentInterval = BeatmapMathHelper.BpmToInterval(currentBpm);

            foreach (var bar in score.Bars) {
                var currentGridPerSignature = bar.GetGridPerSignature();
                var numGrids = bar.GetNumberOfGrids();
                var t = new TimeSpan[numGrids];

                if (allBpmNotes.Any(n => n.Basic.Bar == bar)) {
                    // If there are variant BPM notes, we have to do some math...
                    var bpmNotesInThisBar = allBpmNotes.Where(n => n.Basic.Bar == bar).ToList();

                    bpmNotesInThisBar.Sort((n1, n2) => n1.Basic.IndexInGrid.CompareTo(n2.Basic.IndexInGrid));

                    var bpmNoteIndex = 0;

                    for (var i = 0; i < numGrids; ++i) {
                        if (bpmNoteIndex < bpmNotesInThisBar.Count) {
                            var bpmNote = bpmNotesInThisBar[bpmNoteIndex];

                            if (i == bpmNote.Basic.IndexInGrid) {
                                Debug.Assert(bpmNote.Params != null, "bpmNote.Params != null");

                                // Yes! We have a visitor: a variant BPM note!
                                currentBpm = bpmNote.Params.NewBpm;
                                currentInterval = BeatmapMathHelper.BpmToInterval(currentBpm);

                                ++bpmNoteIndex;
                            }
                        }

                        t[i] = currentTiming;

                        currentTiming += TimeSpan.FromSeconds(currentInterval / currentGridPerSignature);
                    }
                } else {
                    // If there are no variant BPM notes, things get a lot easier.
                    for (var i = 0; i < numGrids; ++i) {
                        t[i] = currentTiming + TimeSpan.FromSeconds(currentInterval * i / currentGridPerSignature);
                    }

                    var currentSignature = bar.GetSignature();

                    currentTiming += TimeSpan.FromSeconds(currentInterval * currentSignature);
                }

                // The timing index should equal corresponding bar index.
                timings.Add(t);
            }

            // Update the HitTiming property of each gaming note.
            foreach (var note in notes) {
                if (!note.Helper.IsGaming) {
                    continue;
                }

                note.Temporary.HitTiming = timings[note.Basic.Bar.Basic.Index][note.Basic.IndexInGrid];
            }

            return timings;
        }

        [NotNull]
        private static CompiledScore Compile([NotNull] Score score, [CanBeNull] TimeSpan? userDefinedEnding) {
            var notes = score.GetAllNotes();

            // First, calculate all timing at the grid lines.
            var allBpmNotes = notes.Where(n => n.Basic.Type == NoteType.VariantBpm).ToArray();
            var timings = new Dictionary<Bar, double[]>();

            var currentTiming = score.Project.Settings.StartTimeOffset;
            var currentBpm = score.Project.Settings.BeatPerMinute;
            var currentInterval = BeatmapMathHelper.BpmToInterval(currentBpm);

            foreach (var bar in score.Bars) {
                var currentGridPerSignature = bar.GetGridPerSignature();
                var numGrids = bar.GetNumberOfGrids();
                var t = new double[numGrids];

                if (allBpmNotes.Any(n => n.Basic.Bar == bar)) {
                    // If there are variant BPM notes, we have to do some math...
                    var bpmNotesInThisBar = allBpmNotes.Where(n => n.Basic.Bar == bar).ToList();

                    bpmNotesInThisBar.Sort((n1, n2) => n1.Basic.IndexInGrid.CompareTo(n2.Basic.IndexInGrid));

                    var bpmNoteIndex = 0;

                    for (var i = 0; i < numGrids; ++i) {
                        if (bpmNoteIndex < bpmNotesInThisBar.Count) {
                            var bpmNote = bpmNotesInThisBar[bpmNoteIndex];

                            if (i == bpmNote.Basic.IndexInGrid) {
                                Debug.Assert(bpmNote.Params != null, "bpmNote.Params != null");

                                // Yes! We have a visitor: a variant BPM note!
                                currentBpm = bpmNote.Params.NewBpm;
                                currentInterval = BeatmapMathHelper.BpmToInterval(currentBpm);

                                ++bpmNoteIndex;
                            }
                        }

                        t[i] = currentTiming;

                        currentTiming += currentInterval / currentGridPerSignature;
                    }
                } else {
                    // If there are no variant BPM notes, things get a lot easier.
                    for (var i = 0; i < numGrids; ++i) {
                        t[i] = currentTiming + currentInterval * i / currentGridPerSignature;
                    }

                    var currentSignature = bar.GetSignature();

                    currentTiming += currentInterval * currentSignature;
                }

                timings[bar] = t;
            }

            // Then, create a list and fill in basic information.
            var compiledNotes = new List<CompiledNote>();
            var noteMap1 = new Dictionary<CompiledNote, Note>();
            var noteMap2 = new Dictionary<Note, CompiledNote>();

            foreach (var note in notes) {
                if (!note.Helper.IsGaming) {
                    continue;
                }

                var compiledNote = new CompiledNote();

                compiledNote.Type = note.Basic.Type;
                compiledNote.HitTiming = timings[note.Basic.Bar][note.Basic.IndexInGrid];
                compiledNote.StartPosition = note.Basic.StartPosition;
                compiledNote.FinishPosition = note.Basic.FinishPosition;
                compiledNote.FlickType = note.Basic.FlickType;
                compiledNote.IsSync = note.Helper.IsSync;

                compiledNotes.Add(compiledNote);

                noteMap1[compiledNote] = note;
                noteMap2[note] = compiledNote;
            }

            compiledNotes.Sort((n1, n2) => n1.HitTiming.CompareTo(n2.HitTiming));

            // ReSharper disable once InconsistentNaming
            // Then, calculate group IDs.
            var currentGroupID = 1;

            foreach (var compiledNote in compiledNotes) {
                if (compiledNote.GroupID != 0) {
                    continue;
                }

                var note = noteMap1[compiledNote];

                if (!note.Helper.IsFlick && !note.Helper.IsSlide) {
                    continue;
                }

                var cn = compiledNote;

                if (note.Helper.IsFlick) {
                    var n = note;

                    while (n != null) {
                        cn.GroupID = currentGroupID;
                        n = n.Editor.NextFlick;

                        if (n != null) {
                            cn = noteMap2[n];
                        }
                    }

                    ++currentGroupID;
                } else if (note.Helper.IsSlide) {
                    var n = note;
                    var n2 = n;

                    while (n != null) {
                        cn.GroupID = currentGroupID;
                        n2 = n;
                        n = n.Editor.NextSlide;

                        if (n != null) {
                            cn = noteMap2[n];
                        }
                    }
                    // A slide group, directly followed by a flick group.
                    if (n2.Helper.HasNextFlick) {
                        n = n2.Editor.NextFlick;

                        while (n != null) {
                            cn = noteMap2[n];
                            cn.GroupID = currentGroupID;
                            n = n.Editor.NextFlick;
                        }
                    }

                    ++currentGroupID;
                }
            }

            // Then, add three key notes. (Keynotes, hahaha.)
            var totalNoteCount = compiledNotes.Count;
            var scoreInfoNote = new CompiledNote();

            scoreInfoNote.Type = NoteType.NoteCount;
            // Here I used a trick. This will run well while violating the meaning of enum.
            scoreInfoNote.FlickType = (NoteFlickType)totalNoteCount;

            var songStartNote = new CompiledNote {
                Type = NoteType.MusicStart
            };
            var endTiming = userDefinedEnding?.TotalSeconds ?? currentTiming;
            var songEndNote = new CompiledNote {
                Type = NoteType.MusicEnd,
                HitTiming = endTiming
            };

            compiledNotes.Insert(0, scoreInfoNote);
            compiledNotes.Insert(1, songStartNote);
            compiledNotes.Add(songEndNote);

            // ReSharper disable once InconsistentNaming
            // Finally, ID them.
            var currentNoteID = 1;

            foreach (var note in compiledNotes) {
                note.ID = currentNoteID;
                ++currentNoteID;
            }

            // We did it! Oh yeah!
            var compiledScore = new CompiledScore(compiledNotes);

            return compiledScore;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CheckBarIndex([NotNull] Score score, int index) {
            var upper = score.Bars.Count - 1;

            if (index < 0 || upper < index) {
                string errorMessage;

                if (upper < 0) {
                    errorMessage = $"The bar with index {index} does not exist. No bar exists.";
                } else {
                    errorMessage = $"The bar with index {index} does not exist. Range is [0, {upper}].";
                }

                throw new ArgumentOutOfRangeException(nameof(index), index, errorMessage);
            }
        }

    }
}
