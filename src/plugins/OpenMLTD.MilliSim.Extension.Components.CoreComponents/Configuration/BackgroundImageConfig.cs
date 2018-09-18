using System.Runtime.Serialization;
using OpenMLTD.MilliSim.Configuration.Entities;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration {
    [DataContract]
    public sealed class BackgroundImageConfig : ConfigBase {

        [DataMember]
        public BackgroundImageConfigData Data { get; set; }

        [DataContract]
        public sealed class BackgroundImageConfigData {

            [DataMember(Name = "bgi")]
            public string BackgroundImage { get; set; }

            [DataMember]
            public BackgroundImageFit Fit { get; set; }

        }

    }
}
