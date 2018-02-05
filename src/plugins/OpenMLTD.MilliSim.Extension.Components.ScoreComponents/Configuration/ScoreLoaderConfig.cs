using OpenMLTD.MilliSim.Configuration.Entities;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration {
    public sealed class ScoreLoaderConfig : ConfigBase {

        public ScoreLoaderConfigData Data { get; set; }

        public sealed class ScoreLoaderConfigData {

            public int ScoreIndex { get; set; }

            public string Title { get; set; }

            [YamlMember(Alias = "scoreobj")]
            public string ScoreFilePath { get; set; }

            [YamlMember(Alias = "offset")]
            public float ScoreOffset { get; set; }

        }

    }
}
