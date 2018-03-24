using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Beatmap {
    public sealed class Note : IStarlightObject {

        internal Note([NotNull] Bar bar)
            : this(bar, Guid.NewGuid()) {
        }

        internal Note([NotNull] Bar bar, Guid id) {
            Basic = new BasicProperties(bar);
            Helper = new HelperProperties(this);
            Editor = new EditorProperties();
            StarlightID = id;
        }

        public Guid StarlightID {
            [DebuggerStepThrough]
            get => Basic.ID;
            [DebuggerStepThrough]
            internal set => Basic.ID = value;
        }

        [NotNull]
        public BasicProperties Basic { get; }

        [NotNull]
        public HelperProperties Helper { get; }

        [NotNull]
        public EditorProperties Editor { get; }

        [CanBeNull]
        public NoteExtraParams Params { get; internal set; }

        public sealed class BasicProperties {

            internal BasicProperties([NotNull] Bar bar) {
                Bar = bar;
            }

            // ReSharper disable once InconsistentNaming
            public Guid ID { get; internal set; }

            [NotNull]
            public Bar Bar { get; }

            public NoteType Type { get; set; } = NoteType.TapOrFlick;

            public int IndexInGrid { get; set; }

            public NotePosition StartPosition { get; set; } = NotePosition.Default;

            public NotePosition FinishPosition { get; set; } = NotePosition.Default;

            public NoteFlickType FlickType { get; set; } = NoteFlickType.None;

        }

        public sealed class HelperProperties {

            internal HelperProperties([NotNull] Note note) {
                _note = note;
            }

            public bool IsGaming {
                [DebuggerStepThrough]
                get => IsTypeGaming(_note.Basic.Type);
            }

            public bool IsSpecial {
                [DebuggerStepThrough]
                get => IsTypeSpecial(_note.Basic.Type);
            }

            public bool IsCgssInternal {
                [DebuggerStepThrough]
                get => IsTypeCgssInternal(_note.Basic.Type);
            }

            public bool IsTap {
                [DebuggerStepThrough]
                get => _note.Basic.Type == NoteType.TapOrFlick && _note.Basic.FlickType == NoteFlickType.None;
            }

            public bool IsSync {
                [DebuggerStepThrough]
                get => HasPrevSync || HasNextSync;
            }

            public bool HasPrevSync {
                [DebuggerStepThrough]
                get => _note.Editor.PrevSync != null;
            }

            public bool HasNextSync {
                [DebuggerStepThrough]
                get => _note.Editor.NextSync != null;
            }

            public bool IsFlick {
                [DebuggerStepThrough]
                get => _note.Basic.FlickType != NoteFlickType.None && _note.Basic.Type == NoteType.TapOrFlick;
            }

            public bool IsFlickStart {
                [DebuggerStepThrough]
                get => IsFlick && HasNextFlick && !HasPrevFlick;
            }

            public bool IsFlickEnd {
                [DebuggerStepThrough]
                get => IsFlick && HasPrevFlick && !HasNextFlick;
            }

            public bool IsFlickMidway {
                [DebuggerStepThrough]
                get => IsFlick && HasPrevFlick && HasNextFlick;
            }

            public bool HasPrevFlick {
                [DebuggerStepThrough]
                get => _note.Editor.PrevFlick != null;
            }

            public bool HasNextFlick {
                [DebuggerStepThrough]
                get => _note.Editor.NextFlick != null;
            }

            public bool IsHoldStart {
                [DebuggerStepThrough]
                get => _note.Basic.Type == NoteType.Hold;
            }

            public bool IsHoldEnd {
                [DebuggerStepThrough]
                get => HasHoldPair && _note.Basic.Type != NoteType.Hold;
            }

            public bool IsHold {
                [DebuggerStepThrough]
                get => IsHoldStart || IsHoldEnd;
            }

            public bool HasHoldPair {
                [DebuggerStepThrough]
                get => _note.Editor.HoldPair != null;
            }

            public bool IsSlide {
                [DebuggerStepThrough]
                get => _note.Basic.Type == NoteType.Slide;
            }

            public bool IsSlideStart {
                [DebuggerStepThrough]
                get => IsSlide && HasNextSlide && !HasPrevSlide;
            }

            public bool IsSlideEnd {
                [DebuggerStepThrough]
                get => IsSlide && HasPrevSlide && !HasNextSlide;
            }

            public bool IsSlideMidway {
                [DebuggerStepThrough]
                get => IsSlide && HasPrevSlide && HasNextSlide;
            }

            public bool HasNextSlide {
                [DebuggerStepThrough]
                get => _note.Editor.NextSlide != null;
            }

            public bool HasPrevSlide {
                [DebuggerStepThrough]
                get => _note.Editor.PrevSlide != null;
            }

            [NotNull]
            private readonly Note _note;

        }

        public sealed class EditorProperties {

            internal EditorProperties() {
            }

            public bool IsSelected { get; internal set; }

            [CanBeNull]
            public Note NextSync { get; internal set; }

            [CanBeNull]
            public Note PrevSync { get; internal set; }

            [CanBeNull]
            public Note NextFlick { get; internal set; }

            [CanBeNull]
            public Note PrevFlick { get; internal set; }

            [CanBeNull]
            public Note NextSlide { get; internal set; }

            [CanBeNull]
            public Note PrevSlide { get; internal set; }

            [CanBeNull]
            public Note HoldPair { get; internal set; }

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TimingThenPositionComparison([NotNull] Note x, [NotNull] Note y) {
            var r = TimingComparison(x, y);

            return r == 0 ? TrackPositionComparison(x, y) : r;
        }

        /// <summary>
        /// Sort by timing. Notes appeared first are placed first.
        /// </summary>
        public static int TimingComparison([NotNull] Note x, [NotNull] Note y) {
            if (x == null) {
                throw new ArgumentNullException(nameof(x));
            }

            if (y == null) {
                throw new ArgumentNullException(nameof(y));
            }

            if (x.Equals(y)) {
                return 0;
            }

            if (x.Basic.Bar.StarlightID != y.Basic.Bar.StarlightID) {
                return x.Basic.Bar.Basic.Index.CompareTo(y.Basic.Bar.Basic.Index);
            }

            var r = x.Basic.IndexInGrid.CompareTo(y.Basic.IndexInGrid);

            if (r == 0 && x.Basic.Type != y.Basic.Type && (x.Basic.Type == NoteType.VariantBpm || y.Basic.Type == NoteType.VariantBpm)) {
                // The Variant BPM note is always placed at the end on the same grid line.
                return x.Basic.Type == NoteType.VariantBpm ? 1 : -1;
            } else {
                return r;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TrackPositionComparison([NotNull] Note n1, [NotNull] Note n2) {
            return ((int)n1.Basic.FinishPosition).CompareTo((int)n2.Basic.FinishPosition);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTypeGaming(NoteType type) {
            switch (type) {
                case NoteType.TapOrFlick:
                case NoteType.Hold:
                case NoteType.Slide:
                    return true;
                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTypeSpecial(NoteType type) {
            switch (type) {
                case NoteType.Avatar:
                case NoteType.VariantBpm:
                    return true;
                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTypeCgssInternal(NoteType type) {
            switch (type) {
                case NoteType.FeverStart:
                case NoteType.FeverEnd:
                case NoteType.MusicStart:
                case NoteType.MusicEnd:
                case NoteType.NoteCount:
                    return true;
                default:
                    return false;
            }
        }

        public static bool operator >([NotNull] Note left, [NotNull] Note right) {
            var r = TimingComparison(left, right);
            return r > 0;
        }

        public static bool operator <([NotNull] Note left, [NotNull] Note right) {
            var r = TimingComparison(left, right);
            return r < 0;
        }

        public override string ToString() {
            return $"Note (ID={StarlightID}, Type={Basic.Type}, Flick={Basic.FlickType}, Row={Basic.IndexInGrid}, Col={Basic.FinishPosition})";
        }

        public sealed class TemporaryProperties {

            public TimeSpan HitTiming { get; internal set; }

            public bool EditorVisible { get; set; }

            // ReSharper disable once InconsistentNaming
            internal Guid PrevFlickNoteID { get; set; }

            // ReSharper disable once InconsistentNaming
            internal Guid NextFlickNoteID { get; set; }

            // ReSharper disable once InconsistentNaming
            internal Guid HoldTargetID { get; set; }

            // ReSharper disable once InconsistentNaming
            internal Guid PrevSlideNoteID { get; set; }

            // ReSharper disable once InconsistentNaming
            internal Guid NextSlideNoteID { get; set; }

        }

        [NotNull]
        public TemporaryProperties Temporary { get; } = new TemporaryProperties();

    }
}
