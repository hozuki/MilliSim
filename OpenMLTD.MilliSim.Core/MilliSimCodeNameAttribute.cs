using System;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core {
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class MilliSimCodeNameAttribute : Attribute {

        public MilliSimCodeNameAttribute([CanBeNull] string codeName) {
            CodeName = codeName ?? DefaultCodeName;
        }

        [NotNull]
        public string CodeName { get; }

        public static readonly string DefaultCodeName = "Unknown";

    }
}
