using OpenMLTD.MilliSim.Configuration.Entities;
using OpenMLTD.MilliSim.Configuration.Primitives;
using OpenMLTD.TheaterDays.Configuration.Primitives;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration {
    public sealed class ComboTextConfig : ConfigBase {

        public ComboTextConfigData Data { get; set; }

        public sealed class ComboTextConfigData {

            public ImageWithBlankEdge Image { get; set; }

            public LayoutValue2D Layout { get; set; }

        }

    }
}
