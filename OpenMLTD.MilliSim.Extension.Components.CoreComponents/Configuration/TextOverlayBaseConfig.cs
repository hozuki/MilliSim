using OpenMLTD.MilliSim.Configuration.Entities;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration {
    public sealed class TextOverlayBaseConfig : ConfigBase {

        public TextOverlayBaseConfigData Data { get; set; }

        public sealed class TextOverlayBaseConfigData {

            public string FontPath { get; set; }

        }

    }
}
