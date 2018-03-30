using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OpenMLTD.TheaterDays.Subsystems.Bvs.Models {
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public sealed class EditReloadRequestParams {

        [CanBeNull]
        public string BeatmapFile { get; set; }

        public int BeatmapIndex { get; set; }

        public float BeatmapOffset { get; set; }

        [CanBeNull]
        public string BackgroundMusicFile { get; set; }

    }
}
