using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Theater.Configuration {
    public sealed class GameClass {

        public int ScoreIndex { get; set; }

        public string Title { get; set; }

        [YamlMember(Alias = "scoreobj")]
        public string ScoreFile { get; set; }

    }
}
