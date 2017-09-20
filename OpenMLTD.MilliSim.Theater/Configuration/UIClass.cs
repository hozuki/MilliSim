using OpenMLTD.MilliSim.Theater.Configuration.Primitives;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Theater.Configuration {
    public sealed class UIClass {

        public UICommonConfig TapPoints { get; set; }

        public UICommonConfig HpGauge { get; set; }

        public UICommonConfig Avatars { get; set; }

        public UICommonConfig ScoreRank { get; set; }

        public ComboClass Combo { get; set; }

        public UICommonConfig NotesLayer { get; set; }

        public UICommonConfig RibbonsLayer { get; set; }

        public SongTitleClass SongTitle { get; set; }

        public UICommonConfig HitRank { get; set; }

        public sealed class SongTitleClass {

            public LayoutValue2D Layout { get; set; }

            public float FontSize { get; set; }

            [YamlMember(Alias = "text_stroke_width")]
            public float StrokeWidth { get; set; }

        }

        public sealed class ComboClass {

            public UICommonConfig Aura { get; set; }

            public UICommonConfig Text { get; set; }

            public UICommonConfig Numbers { get; set; }

        }

        public sealed class UICommonConfig {

            public LayoutValue2D Layout { get; set; }

            public PercentOrRealValue Opacity { get; set; }

        }

    }
}
