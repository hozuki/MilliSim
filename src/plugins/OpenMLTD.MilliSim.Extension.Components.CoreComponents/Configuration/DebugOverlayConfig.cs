using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Configuration.Entities;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration {
    public sealed class DebugOverlayConfig : ConfigBase {

        public DebugOverlayConfigData Data { get; set; }

        public sealed class DebugOverlayConfigData {

            public bool Visible { get; set; }

            public Color TextFill { get; set; }

            public float FontSize { get; set; }

        }

    }
}
