using OpenMLTD.MilliSim.Configuration.Entities;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration {
    public sealed class BackgroundImageConfig : ConfigBase {

        public BackgroundImageConfigData Data { get; set; }

        public sealed class BackgroundImageConfigData {

            [YamlMember(Alias = "bgi")]
            public string BackgroundImage { get; set; }

            [YamlMember]
            public BackgroundImageFit Fit { get; set; }

        }

    }
}
