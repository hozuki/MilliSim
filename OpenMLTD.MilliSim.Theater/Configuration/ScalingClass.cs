using System.Drawing;

namespace OpenMLTD.MilliSim.Theater.Configuration {
    public sealed class ScalingClass {

        public SizeF Base { get; set; }

        public SizeF TapPoint { get; set; }

        public SizeF TapBarChain { get; set; }

        public SizeF TapBarNode { get; set; }

        public NoteScaling Note { get; set; } = new NoteScaling();

        public NoteScaling SpecialNote { get; set; } = new NoteScaling();

        public SizeF SyncLine { get; set; }

        public NoteScaling VisualNoteSmall { get; set; } = new NoteScaling();

        public NoteScaling VisualNoteLarge { get; set; } = new NoteScaling();

        public sealed class NoteScaling {

            public SizeF Start { get; set; }

            public SizeF End { get; set; }

        }

    }
}
