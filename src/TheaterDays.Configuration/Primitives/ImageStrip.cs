using YamlDotNet.Serialization;

namespace OpenMLTD.TheaterDays.Configuration.Primitives {
    public sealed class ImageStrip {

        public string File { get; set; }

        public int Count { get; set; }

        public ImageStripOrientation Orientation { get; set; }

        [YamlMember(Alias = "blank_edge")]
        public SimpleRectangleF UnitBlankEdge { get; set; }

    }
}
