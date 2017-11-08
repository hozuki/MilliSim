using System.Drawing;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Configuration.Primitives {
    public sealed class ImageWithBlankEdge {

        [YamlMember(Alias = "file")]
        public string FileName { get; set; }

        public SimpleRectangleF BlankEdge { get; set; }

        /// <summary>
        /// The center point, regardless of <see cref="BlankEdge"/> value.
        /// </summary>
        public PointF Center { get; set; }

    }
}
