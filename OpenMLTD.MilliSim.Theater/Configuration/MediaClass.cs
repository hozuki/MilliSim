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
        public int BackgroundMusicVolume { get; set; }

        [YamlMember(Alias = "bga_volume")]
        public int BackgroundAnimationVolume { get; set; }

        [YamlMember(Alias = "sfx_volume")]
        public int SoundEffectsVolume { get; set; }

    }
}
