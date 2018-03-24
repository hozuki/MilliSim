using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Beatmap.Extensions {
    public static class NoteExtensions {

        public static void SetSpecialType([NotNull] this Note @this, NoteType type) {
            switch (type) {
                case NoteType.VariantBpm:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }

            @this.Basic.Type = type;
        }


        public static void FixSyncWhenAdded([NotNull] this Note @this) {
            // TODO
            if (!@this.Helper.IsGaming) {
                return;
            }

            @this.RemoveSync();

            Note prev = null, next = null;

            foreach (var note in @this.Basic.Bar.Notes) {
                if (note == @this) {
                    continue;
                }

                if (!note.Helper.IsGaming) {
                    continue;
                }

                if (note.Basic.IndexInGrid != @this.Basic.IndexInGrid) {
                    continue;
                }

                if (note.Basic.FinishPosition < @this.Basic.FinishPosition) {
                    if (prev == null || prev.Basic.FinishPosition < note.Basic.FinishPosition) {
                        prev = note;
                    }
                } else {
                    if (next == null || note.Basic.FinishPosition < next.Basic.FinishPosition) {
                        next = note;
                    }
                }
            }

            NoteHelper.MakeSync(prev, @this);
            NoteHelper.MakeSync(@this, next);
        }

        public static void ResetToTap([NotNull] this Note note) {
            ResetToTap(note, false);
        }

        public static void ResetToTap([NotNull] this Note note, bool keepSync) {
            if (!keepSync) {
                // Fix sync notes.
                note.RemoveSync();
            }

            // Fix flick notes.
            Note prevFlick = note.Editor.PrevFlick, nextFlick = note.Editor.NextFlick;

            // New strategy: force split the flick group into two. Reconnect if the user needs to.
            if (prevFlick != null) {
                prevFlick.Editor.NextFlick = null;
            }

            if (nextFlick != null) {
                nextFlick.Editor.PrevFlick = null;
                nextFlick.Editor.PrevSlide = null;
            }

            if (prevFlick != null) {
                if (!prevFlick.Helper.HasPrevFlick && !prevFlick.Helper.HasNextFlick) {
                    prevFlick.Basic.FlickType = NoteFlickType.None;
                }
            }

            if (nextFlick != null) {
                if (!nextFlick.Helper.HasPrevFlick && !nextFlick.Helper.HasNextFlick) {
                    nextFlick.Basic.FlickType = NoteFlickType.None;
                }
            }

            // Actually deleting a flick note only influences the flick note before it (prevFlick).
            if (prevFlick != null) {
                FixFlickDirections(prevFlick);
            }

            if (nextFlick != null) {
                FixFlickDirections(nextFlick);
            }

            // Fix slide notes.
            var prevSlide = note.Editor.PrevSlide;
            var nextSlide = note.Editor.NextSlide;

            if (prevSlide != null) {
                prevSlide.Editor.NextSlide = null;
                prevSlide.Editor.NextFlick = null;
            }

            if (nextSlide != null) {
                nextSlide.Editor.PrevSlide = null;
            }

            if (prevSlide != null) {
                if (!prevSlide.Helper.HasPrevSlide && !prevSlide.Helper.HasNextSlide) {
                    prevSlide.Basic.Type = NoteType.TapOrFlick;
                }

                if (prevSlide.Helper.HasPrevSlide) {
                    // This case only happens when we delete the flick note of a slide-flick relation.
                    // Therefore, prevSlide's flick type is definitely None.
                    prevSlide.Basic.FlickType = NoteFlickType.None;
                }
            }

            if (nextSlide != null) {
                if (!nextSlide.Helper.HasPrevSlide && !nextSlide.Helper.HasNextSlide) {
                    nextSlide.Basic.Type = NoteType.TapOrFlick;
                    nextSlide.Basic.FlickType = NoteFlickType.None;
                }
            }

            if (prevSlide != null) {
                FixFlickDirections(prevSlide);
            }

            if (nextSlide != null) {
                FixFlickDirections(nextSlide);
            }

            // Fix hold notes.
            if (note.Helper.HasHoldPair) {
                var holdPair = note.Editor.HoldPair;

                Debug.Assert(holdPair != null, nameof(holdPair) + " != null");

                holdPair.Editor.HoldPair = null;

                if (holdPair.Helper.IsHoldStart) {
                    holdPair.Basic.Type = NoteType.TapOrFlick;
                } else {
                    if (!holdPair.Helper.HasNextFlick) {
                        holdPair.Basic.FlickType = NoteFlickType.None;
                    }
                }
            }

            var e = note.Editor;

            if (!keepSync) {
                e.PrevSync = e.NextSync = null;
            }

            e.HoldPair = e.NextFlick = e.PrevFlick = e.NextSlide = e.PrevSlide = null;

            var b = note.Basic;

            b.Type = NoteType.TapOrFlick;
            b.FlickType = NoteFlickType.None;
        }

        [DebuggerStepThrough]
        public static void EditorToggleSelected([NotNull] this Note note) {
            note.Editor.IsSelected = !note.Editor.IsSelected;
        }

        [DebuggerStepThrough]
        public static void EditorSelect([NotNull] this Note note) {
            note.Editor.IsSelected = true;
        }

        [DebuggerStepThrough]
        public static void EditorUnselect([NotNull] this Note note) {
            note.Editor.IsSelected = false;
        }

        [DebuggerStepThrough]
        internal static void SetPrevSyncTargetInternal([NotNull] this Note @this, [CanBeNull] Note prev) {
            @this.Editor.PrevSync = prev;
        }

        [DebuggerStepThrough]
        internal static void SetNextSyncTargetInternal([NotNull] this Note @this, [CanBeNull] Note next) {
            @this.Editor.NextSync = next;
        }

        private static void RemoveSync([NotNull] this Note note) {
            /*
             * Before:
             *     ... <==> prev <==> this <==> next <==> ...
             *
             * After:
             *     ... <==> prev <============> next <==> ...
             *                        this
             */
            note.Editor.PrevSync?.SetNextSyncTargetInternal(note.Editor.NextSync);
            note.Editor.NextSync?.SetPrevSyncTargetInternal(note.Editor.PrevSync);
            note.SetPrevSyncTargetInternal(null);
            note.SetNextSyncTargetInternal(null);
        }

        private static void FixFlickDirections([NotNull] this Note flickNoteInGroup) {
            if (!flickNoteInGroup.Helper.HasPrevFlick && !flickNoteInGroup.Helper.HasNextFlick) {
                return;
            }

            // Find the first flick note in this group.
            var firstFlick = flickNoteInGroup;

            while (firstFlick != null && firstFlick.Helper.HasPrevFlick) {
                firstFlick = firstFlick.Editor.PrevFlick;
            }

            // Set the directions.
            var currentFlick = firstFlick;
            var nextFlick = currentFlick?.Editor.NextFlick;

            if (nextFlick != null) {
                do {
                    currentFlick.Basic.FlickType = nextFlick.Basic.FinishPosition > currentFlick.Basic.FinishPosition ? NoteFlickType.Right : NoteFlickType.Left;
                    // Trick. So we don't need to track the previous flick to decide the flick direction of the last note of the group.
                    nextFlick.Basic.FlickType = currentFlick.Basic.FlickType;
                    currentFlick = nextFlick;
                    nextFlick = currentFlick.Editor.NextFlick;
                } while (nextFlick != null);
            }
        }

    }
}
