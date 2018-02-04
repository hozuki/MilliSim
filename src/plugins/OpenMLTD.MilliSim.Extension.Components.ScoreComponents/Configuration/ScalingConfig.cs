using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Configuration.Entities;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration {
    public sealed class ScalingConfig : ConfigBase {

        public ScalingConfigData Data { get; set; }

        public sealed class ScalingConfigData {

            public Vector2 Base { get; set; }

            public SizableScaling TapPoint { get; set; }

            public Vector2 TapBarChain { get; set; }

            public Vector2 TapBarNode { get; set; }

            public SizableScaling Note { get; set; }

            public SizableScaling SpecialNote { get; set; }

            public Vector2 SyncLine { get; set; }

            public SizableScaling VisualNoteSmall { get; set; }

            public SizableScaling VisualNoteLarge { get; set; }

            public SizableScaling SpecialNoteAura { get; set; }

            public Vector2 SpecialNoteSocket { get; set; }

            public Vector2 HitRank { get; set; }

            public Vector2 AvatarBorder { get; set; }

            public Vector2 ComboAura { get; set; }

            public Vector2 ComboText { get; set; }

            public Vector2 ComboNumber { get; set; }

        }

        public sealed class SizableScaling {

            public Vector2 Start { get; set; }

            public Vector2 End { get; set; }

        }

    }
}
