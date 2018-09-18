using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Configuration.Entities;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration {
    [DataContract]
    public sealed class DebugOverlayConfig : ConfigBase {

        [DataMember]
        public DebugOverlayConfigData Data { get; set; }

        [DataContract]
        public sealed class DebugOverlayConfigData {

            [DataMember]
            public bool Visible { get; set; }

            [DataMember]
            public Color TextFill { get; set; }

            [DataMember]
            public float FontSize { get; set; }

        }

    }
}
