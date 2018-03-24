using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Editor.Serialization {
    partial class SldprojV2Reader {

        [JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
        private sealed class BarParamsV2 {

            [Obsolete("This property is not used since v0.5.0 alpha. Please consider Note with Note.Type == VariantBpm instead.")]
            public double? UserDefinedBpm { get; set; }

            public int? UserDefinedGridPerSignature { get; set; }

            public int? UserDefinedSignature { get; set; }

        }

    }
}
