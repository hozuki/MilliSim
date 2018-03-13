using OpenMLTD.MilliSim.Configuration.Entities;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration {
    public sealed class SyncTimerConfig : ConfigBase {

        public SyncTimerConfigData Data { get; set; }

        public sealed class SyncTimerConfigData {

            public TimerSyncTarget SyncTarget { get; set; }

            public TimerSyncMethod SyncMethod { get; set; }

        }

    }
}
