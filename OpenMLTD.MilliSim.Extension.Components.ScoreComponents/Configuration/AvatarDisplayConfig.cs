using OpenMLTD.MilliSim.Configuration.Entities;
using OpenMLTD.MilliSim.Configuration.Primitives;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration {
    public sealed class AvatarDisplayConfig : ConfigBase {

        public AvatarDisplayConfigData Data { get; set; }

        public sealed class AvatarDisplayConfigData {

            public ImageWithBlankEdge[] Images { get; set; }

            public LayoutValue2D Layout { get; set; }

        }

    }
}
