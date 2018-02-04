using OpenMLTD.MilliSim.Configuration.Entities;
using OpenMLTD.MilliSim.Configuration.Primitives;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration {
    public sealed class TapPointsConfig : ConfigBase {

        public TapPointsConfigData Data { get; set; }

        public sealed class TapPointsConfigData {

            public TapPointsImageClass Images { get; set; }

            public LayoutValue2D Layout { get; set; }

            public PercentOrRealValue Opacity { get; set; }

        }

        public sealed class TapPointsImageClass {

            public ImageWithBlankEdge TapPoint { get; set; }

            public ImageWithBlankEdge TapBarChain { get; set; }

            public ImageWithBlankEdge TapBarNode { get; set; }

        }

    }
}
