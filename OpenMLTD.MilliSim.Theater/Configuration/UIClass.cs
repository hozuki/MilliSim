using OpenMLTD.MilliSim.Theater.Configuration.Primitives;

namespace OpenMLTD.MilliSim.Theater.Configuration {
    public sealed class UIClass {

        public UICommonConfig TapPoints { get; set; }

        public UICommonConfig HpGauge { get; set; }

        public UICommonConfig Avatars { get; set; }

        public UICommonConfig ScoreRank { get; set; }

        public UICommonConfig ComboCounter { get; set; }

        public UICommonConfig NotesLayer { get; set; }

        public UICommonConfig RibbonsLayer { get; set; }

        public UICommonConfig Title { get; set; }

        public UICommonConfig HitRank { get; set; }

        public sealed class UICommonConfig {

            public LayoutCoordinate2D Layout { get; set; }

            public PercentOrRealValue Opacity { get; set; }

        }

    }
}
