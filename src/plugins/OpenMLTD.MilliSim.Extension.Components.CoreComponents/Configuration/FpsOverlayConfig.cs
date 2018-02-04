using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Configuration.Entities;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration {
    public sealed class FpsOverlayConfig : ConfigBase {

        public FpsOverlayConfigData Data { get; set; }

        public sealed class FpsOverlayConfigData {

            public bool Visible { get; set; }

            public Color TextFill { get; set; }

            public float FontSize { get; set; }

        }

    }
}
