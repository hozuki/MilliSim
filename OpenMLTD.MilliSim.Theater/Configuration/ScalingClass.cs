using System.Drawing;

namespace OpenMLTD.MilliSim.Theater.Configuration {
    public sealed class ScalingClass {

        public SizeF Base { get; set; }

        public SizableScaling TapPoint { get; set; }

        public SizeF TapBarChain { get; set; }

        public SizeF TapBarNode { get; set; }

        public SizableScaling Note { get; set; }

        public SizableScaling SpecialNote { get; set; }

        public SizeF SyncLine { get; set; }

        public SizableScaling VisualNoteSmall { get; set; }

        public SizableScaling VisualNoteLarge { get; set; }

        public SizableScaling SpecialNoteAura { get; set; }

        public SizeF SpecialNoteSocket { get; set; }

        public sealed class SizableScaling {

            public SizeF Start { get; set; }

            public SizeF End { get; set; }

        }

    }
}
