using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OpenMLTD.TheaterDays.Subsystems.Bvs.Models {
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public sealed class EditReloadRequestParams {

        [CanBeNull]
        public string ScoreFile { get; set; }

        public int ScoreIndex { get; set; }

        public float ScoreOffset { get; set; }

        [CanBeNull]
        public string BackgroundMusic { get; set; }

    }
}
