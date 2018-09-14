using OpenMLTD.MilliSim.Configuration.Entities;
using OpenMLTD.MilliSim.Configuration.Primitives;
using OpenMLTD.TheaterDays.Configuration.Primitives;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration {
    public sealed class HitRankAnimationConfig : ConfigBase {

        public HitRankAnimationConfigData Data { get; set; }

        public sealed class HitRankAnimationConfigData {

            public ImageStrip Images { get; set; }

            public LayoutValue2D Layout { get; set; }

        }

    }
}
