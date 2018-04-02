using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenMLTD.TheaterDays.Subsystems.Bvs.Models.Proposals;

namespace OpenMLTD.TheaterDays.Subsystems.Bvs.Models {
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public sealed class GeneralSimInitializeRequestParams {

        [JsonConstructor]
        internal GeneralSimInitializeRequestParams() {
        }

        [NotNull, ItemNotNull]
        public SupportedFormatDescriptor[] SupportedFormats { get; set; }

    }
}
