using OpenMLTD.MilliSim.Theater.Configuration;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Theater {
    public sealed class ApplicationSettings {

        public MetadataClass Metadata { get; set; }

        public WindowClass Window { get; set; }

        public ImagesClass Images { get; set; }

        public MediaClass Media { get; set; }

        [YamlMember(Alias = "ui")]
        public UIClass UI { get; set; }

        public StyleClass Style { get; set; }

        public GameClass Game { get; set; }

        [YamlMember(Alias = "system_ui")]
        public SystemUIClass SystemUI { get; set; }

        public SfxClass Sfx { get; set; }

        [YamlMember(Alias = "localization")]
        public LocalStringsClass LocalStrings { get; set; }

        public ScalingClass Scaling { get; set; }

    }
}
