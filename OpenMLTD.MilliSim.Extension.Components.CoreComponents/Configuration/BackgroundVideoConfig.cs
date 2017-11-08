using OpenMLTD.MilliSim.Configuration.Entities;
using OpenMLTD.MilliSim.Configuration.Primitives;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration {
    public sealed class BackgroundVideoConfig : ConfigBase {

        public BackgroundVideoConfigData Data { get; set; }

        public sealed class BackgroundVideoConfigData {

            [YamlMember(Alias = "video")]
            public string BackgroundVideo { get; set; }

            [YamlMember(Alias = "video_volume")]
            public PercentOrRealValue BackgroundVideoVolume { get; set; }

        }

    }
}
