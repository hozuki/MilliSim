using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Configuration.Entities;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration {
    [DataContract]
    public sealed class FpsOverlayConfig : ConfigBase {

        [DataMember]
        public FpsOverlayConfigData Data { get; set; }

        [DataContract]
        public sealed class FpsOverlayConfigData {

            [DataMember]
            public bool Visible { get; set; }

            [DataMember]
            public Color TextFill { get; set; }

            [DataMember]
            public float FontSize { get; set; }

        }

    }
}
