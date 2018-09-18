using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Configuration.Entities;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration {
    [DataContract]
    public sealed class SyncTimerOverlayConfig : ConfigBase {

        [DataMember]
        public SyncTimerOverlayConfigData Data { get; set; }

        [DataContract]
        public sealed class SyncTimerOverlayConfigData {

            [DataMember]
            public bool Visible { get; set; }

            [DataMember]
            public Color TextFill { get; set; }

            [DataMember]
            public float FontSize { get; set; }

        }

    }
}
