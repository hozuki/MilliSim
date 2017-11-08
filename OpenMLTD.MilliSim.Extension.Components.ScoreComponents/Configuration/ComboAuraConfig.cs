using OpenMLTD.MilliSim.Configuration.Entities;
using OpenMLTD.MilliSim.Configuration.Primitives;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration {
    public sealed class ComboAuraConfig : ConfigBase {

        public ComboAuraConfigData Data { get; set; }

        public sealed class ComboAuraConfigData {

            public ImageWithBlankEdge Image { get; set; }

            public LayoutValue2D Layout { get; set; }

        }

    }
}
