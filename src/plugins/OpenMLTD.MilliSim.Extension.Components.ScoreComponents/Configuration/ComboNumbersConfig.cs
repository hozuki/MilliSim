using OpenMLTD.MilliSim.Configuration.Entities;
using OpenMLTD.MilliSim.Configuration.Primitives;
using OpenMLTD.TheaterDays.Configuration.Primitives;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration {
    public sealed class ComboNumbersConfig : ConfigBase {

        public ComboNumbersConfigData Data { get; set; }

        public sealed class ComboNumbersConfigData {

            public ImageStrip Images { get; set; }

            public LayoutValue2D Layout { get; set; }

        }

    }
}
