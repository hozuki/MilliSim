using OpenMLTD.MilliSim.Configuration.Entities;
using OpenMLTD.MilliSim.Configuration.Primitives;
using OpenMLTD.TheaterDays.Configuration.Primitives;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration {
    public sealed class RibbonConfig : ConfigBase {

        public RibbonConfigData Data { get; set; }

        public sealed class RibbonConfigData {

            public ImageWithBlankEdge Image { get; set; }

            public PercentOrRealValue Opacity { get; set; }

        }

    }
}
