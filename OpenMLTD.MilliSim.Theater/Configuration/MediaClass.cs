using OpenMLTD.MilliSim.Theater.Configuration.Primitives;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Theater.Configuration {
    public sealed class MediaClass {

        [YamlMember(Alias = "bgm")]
        public string BackgroundMusic { get; set; }

        [YamlMember(Alias = "bga")]
        public string BackgroundAnimation { get; set; }

        [YamlMember(Alias = "bgi")]
        public string BackgroundImage { get; set; }

        [YamlMember(Alias = "bgm_volume")]
        public PercentOrRealValue BackgroundMusicVolume { get; set; } = new PercentOrRealValue(1f, false);

        [YamlMember(Alias = "bga_volume")]
        public PercentOrRealValue BackgroundAnimationVolume { get; set; } = new PercentOrRealValue(1f, false);

        [YamlMember(Alias = "sfx_volume")]
        public PercentOrRealValue SoundEffectsVolume { get; set; } = new PercentOrRealValue(1f, false);

    }
}
