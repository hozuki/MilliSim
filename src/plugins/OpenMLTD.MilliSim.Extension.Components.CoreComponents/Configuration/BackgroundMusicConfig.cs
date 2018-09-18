using System.Runtime.Serialization;
using OpenMLTD.MilliSim.Configuration.Entities;
using OpenMLTD.MilliSim.Configuration.Primitives;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration {
    [DataContract]
    public sealed class BackgroundMusicConfig : ConfigBase {

        [DataMember]
        public BackgroundMusicConfigData Data { get; set; }

        [DataContract]
        public sealed class BackgroundMusicConfigData {

            [DataMember(Name = "bgm")]
            public string BackgroundMusic { get; set; }

            [DataMember(Name = "bgm_volume")]
            public PercentOrRealValue BackgroundMusicVolume { get; set; }

        }

    }
}
