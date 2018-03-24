using System;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Beatmap {
    public sealed class BarParams {

        [Obsolete("This property is ignored since v0.5.0 alpha. Please consider Note with Note.Type == VariantBpm instead.")]
        [CanBeNull]
        public double? UserDefinedBpm { get; set; }

        [CanBeNull]
        public int? UserDefinedGridPerSignature { get; set; }

        [CanBeNull]
        public int? UserDefinedSignature { get; set; }

#pragma warning disable 618
        internal bool CanBeSquashed => UserDefinedBpm == null && UserDefinedGridPerSignature == null && UserDefinedSignature == null;
#pragma warning restore 618

    }
}
