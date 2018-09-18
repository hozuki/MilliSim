using System.Runtime.Serialization;
using OpenMLTD.MilliSim.Configuration.Entities;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration {
    [DataContract]
    public sealed class SyncTimerConfig : ConfigBase {

        [DataMember]
        public SyncTimerConfigData Data { get; set; }

        [DataContract]
        public sealed class SyncTimerConfigData {

            [DataMember]
            public TimerSyncTarget SyncTarget { get; set; }

            [DataMember]
            public TimerSyncMethod SyncMethod { get; set; }

        }

    }
}
