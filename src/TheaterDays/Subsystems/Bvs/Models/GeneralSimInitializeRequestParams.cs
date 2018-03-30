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

        public GeneralSimInitializeRequestParamsData Data { get; set; }

        [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public class GeneralSimInitializeRequestParamsData {

            [JsonConstructor]
            internal GeneralSimInitializeRequestParamsData() {
            }

            [NotNull, ItemNotNull]
            public SupportedFormatDescriptor[] SupportedFormats { get; set; }

        }

    }
}
