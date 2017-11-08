using System.Drawing;
using OpenMLTD.MilliSim.Configuration.Entities;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration {
    public sealed class ScalingConfig : ConfigBase {

        public ScalingConfigData Data { get; set; }

        public sealed class ScalingConfigData {

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

            public SizeF HitRank { get; set; }

            public SizeF AvatarBorder { get; set; }

            public SizeF ComboAura { get; set; }

            public SizeF ComboText { get; set; }

            public SizeF ComboNumber { get; set; }

        }

        public sealed class SizableScaling {

            public SizeF Start { get; set; }

            public SizeF End { get; set; }

        }

    }
}
