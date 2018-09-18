using System.Runtime.Serialization;
using OpenMLTD.MilliSim.Configuration.Entities;
using OpenMLTD.MilliSim.Configuration.Primitives;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration {
    [DataContract]
    public sealed class BackgroundVideoConfig : ConfigBase {

        [DataMember]
        public BackgroundVideoConfigData Data { get; set; }

        [DataContract]
        public sealed class BackgroundVideoConfigData {

            [DataMember(Name = "video")]
            public string BackgroundVideo { get; set; }

            [DataMember(Name = "video_volume")]
            public PercentOrRealValue BackgroundVideoVolume { get; set; }

        }

    }
}
