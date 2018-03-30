using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace OpenMLTD.TheaterDays.Subsystems.Bvs.Models.Proposals {
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public sealed class SelectedFormatDescriptor {

        [NotNull]
        public string Game { get; set; }

        [JsonProperty("id")]
        [NotNull]
        public string FormatId { get; set; }

        [NotNull]
        public string Version { get; set; }

        [CanBeNull]
        public JToken Extra { get; set; }

    }
}
