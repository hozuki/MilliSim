using OpenMLTD.MilliSim.Core.Entities;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Theater.Configuration {
    public sealed class GameClass {

        public Difficulty Difficulty { get; set; }

        public string Title { get; set; }

        [YamlMember(Alias = "scoreobj")]
        public string ScoreFile { get; set; }

    }
}
