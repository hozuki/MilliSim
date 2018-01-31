using System.Drawing;
using OpenMLTD.MilliSim.Configuration.Entities;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration {
    public sealed class SyncTimerOverlayConfig : ConfigBase {

        public SyncTimerOverlayConfigData Data { get; set; }

        public sealed class SyncTimerOverlayConfigData {

            public bool Visible { get; set; }

            public Color TextFill { get; set; }

            public float FontSize { get; set; }

        }

    }
}
