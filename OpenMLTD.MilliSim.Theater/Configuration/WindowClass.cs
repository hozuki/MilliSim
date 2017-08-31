using OpenMLTD.MilliSim.Theater.Configuration.Primitives;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Theater.Configuration {
    public sealed class WindowClass {

        public WindowOrientation Orientation { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public FontsClass Fonts { get; set; }

        public sealed class FontsClass {

            [YamlMember(Alias = "ui")]
            public string UI { get; set; }

            public string SongTitle { get; set; }

        }

    }
}
