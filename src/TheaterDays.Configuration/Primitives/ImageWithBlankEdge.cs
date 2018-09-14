using Microsoft.Xna.Framework;
using YamlDotNet.Serialization;

namespace OpenMLTD.TheaterDays.Configuration.Primitives {
    public sealed class ImageWithBlankEdge {

        [YamlMember(Alias = "file")]
        public string FileName { get; set; }

        public SimpleRectangleF BlankEdge { get; set; }

        /// <summary>
        /// The center point, regardless of <see cref="BlankEdge"/> value.
        /// </summary>
        public Vector2 Center { get; set; }

    }
}
