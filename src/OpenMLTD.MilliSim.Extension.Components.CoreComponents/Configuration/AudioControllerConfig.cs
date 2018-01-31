using OpenMLTD.MilliSim.Configuration.Entities;
using OpenMLTD.MilliSim.Configuration.Primitives;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration {
    public sealed class AudioControllerConfig : ConfigBase {

        public AudioControllerConfigData Data { get; set; }

        public sealed class AudioControllerConfigData {

            [YamlMember(Alias = "bgm")]
            public string BackgroundMusic { get; set; }

            [YamlMember(Alias = "bgm_volume")]
            public PercentOrRealValue BackgroundMusicVolume { get; set; }

            public SfxData Sfx { get; set; }

            [YamlMember(Alias = "sfx_volume")]
            public PercentOrRealValue SfxVolume { get; set; }

        }

        public sealed class SfxData {

            public NoteSfxGroup Tap { get; set; }

            public NoteSfxGroup Hold { get; set; }

            public NoteSfxGroup Flick { get; set; }

            public NoteSfxGroup Slide { get; set; }

            public string SlideHold { get; set; }

            public string HoldHold { get; set; }

            public NoteSfxGroup SlideEnd { get; set; }

            public NoteSfxGroup HoldEnd { get; set; }

            public NoteSfxGroup Special { get; set; }

            public string SpecialEnd { get; set; }

            public string SpecialHold { get; set; }

            public string[] Shouts { get; set; }

        }

        public sealed class NoteSfxGroup {

            public string Perfect { get; set; }

            public string Great { get; set; }

            public string Nice { get; set; }

            public string Bad { get; set; }

            public string Miss { get; set; }

        }

    }
}
