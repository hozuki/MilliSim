using OpenMLTD.MilliSim.Configuration.Entities;
using OpenMLTD.MilliSim.Configuration.Primitives;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration {
    public sealed class BackgroundMusicConfig : ConfigBase {

        public BackgroundMusicConfigData Data { get; set; }

        public sealed class BackgroundMusicConfigData {

            [YamlMember(Alias = "bgm")]
            public string BackgroundMusic { get; set; }

            [YamlMember(Alias = "bgm_volume")]
            public PercentOrRealValue BackgroundMusicVolume { get; set; }

        }

    }
}
