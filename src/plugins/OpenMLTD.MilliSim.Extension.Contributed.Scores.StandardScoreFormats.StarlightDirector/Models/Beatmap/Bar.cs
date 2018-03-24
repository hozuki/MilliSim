using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Beatmap {
    public sealed class Bar : IStarlightObject {

        internal Bar([NotNull] Score score, int index)
            : this(score, index, Guid.NewGuid()) {
        }

        internal Bar([NotNull] Score score, int index, Guid id) {
            var basic = new BarBasicProperties(score);
            basic.ID = id;
            basic.Index = index;

            Basic = basic;
            Helper = new BarHelperProperties(this);
        }

        [NotNull]
        public BarBasicProperties Basic { get; }

        [NotNull]
        public BarEditorProperties Editor { get; } = new BarEditorProperties();

        [NotNull]
        public BarHelperProperties Helper { get; }

        [NotNull]
        public BarTemporaryProperties Temporary { get; } = new BarTemporaryProperties();

        [CanBeNull]
        public BarParams Params { get; internal set; }

        [NotNull, ItemNotNull]
        public List<Note> Notes { get; } = new List<Note>();

        public Guid StarlightID {
            [DebuggerStepThrough]
            get { return Basic.ID; }
        }

        public override string ToString() {
            return $"Bar (Index={Basic.Index}, ID={StarlightID}, Notes={Notes.Count}, Start={Temporary.StartTime:mm\\:ss\\.fff})";
        }

        public sealed class BarBasicProperties {

            internal BarBasicProperties(Score score) {
                Score = score;
            }

            public Score Score { get; }

            public int Index { get; internal set; }

            public Guid ID { get; internal set; }

        }

        public sealed class BarEditorProperties {

            internal BarEditorProperties() {
            }

            public bool IsSelected { get; internal set; }

        }

        public sealed class BarHelperProperties {

            internal BarHelperProperties(Bar bar) {
                _bar = bar;
            }

            public bool HasAnyNote {
                [DebuggerStepThrough]
                get { return _bar.Notes.Count > 0; }
            }

            private readonly Bar _bar;

        }

        public sealed class BarTemporaryProperties {

            internal BarTemporaryProperties() {
            }

            public TimeSpan StartTime { get; internal set; }

        }

    }
}
