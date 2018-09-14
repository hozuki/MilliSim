using OpenMLTD.MilliSim.Configuration.Entities;
using OpenMLTD.MilliSim.Configuration.Primitives;
using OpenMLTD.TheaterDays.Configuration.Primitives;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration {
    public sealed class SongTitleConfig : ConfigBase {

        public SongTitleConfigData Data { get; set; }

        public sealed class SongTitleConfigData {

            public LayoutValue2D Layout { get; set; }

            public string FontFile { get; set; }

            public float FontSize { get; set; }

            public float TextStrokeWidth { get; set; }

            public SongTitleAnimation Animation { get; set; }

        }

        public sealed class SongTitleAnimation {

            public AnimationGroup Appear { get; set; }

            public AnimationGroup Reappear { get; set; }

            public sealed class AnimationGroup {

                public double Enter { get; set; }

                public double FadeIn { get; set; }

                public double Hold { get; set; }

                public double FadeOut { get; set; }

            }

        }

    }
}
