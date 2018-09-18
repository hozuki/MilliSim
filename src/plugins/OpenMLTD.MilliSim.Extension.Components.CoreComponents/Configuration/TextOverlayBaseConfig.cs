using System.Runtime.Serialization;
using OpenMLTD.MilliSim.Configuration.Entities;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration {
    [DataContract]
    public sealed class TextOverlayBaseConfig : ConfigBase {

        [DataMember]
        public TextOverlayBaseConfigData Data { get; set; }

        [DataContract]
        public sealed class TextOverlayBaseConfigData {

            [DataMember]
            public string FontPath { get; set; }

        }

    }
}
