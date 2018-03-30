using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenMLTD.TheaterDays.Subsystems.Bvs.Models.Proposals;

namespace OpenMLTD.TheaterDays.Subsystems.Bvs.Models {
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public sealed class GeneralSimInitializeResponseResult {

        [JsonConstructor]
        internal GeneralSimInitializeResponseResult() {
        }

        [CanBeNull]
        public SelectedFormatDescriptor SelectedFormat { get; set; }

    }
}
