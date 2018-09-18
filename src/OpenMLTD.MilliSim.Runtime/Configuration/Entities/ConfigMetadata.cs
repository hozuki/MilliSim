using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Configuration.Entities {
    /// <summary>
    /// Metadata entity for configuration items.
    /// </summary>
    [DataContract]
    public sealed class ConfigMetadata {

        [DataMember]
        public int Version { get; set; }

        [DataMember]
        [NotNull]
        public string Type { get; set; } = string.Empty;

        [DataMember]
        [NotNull]
        public string AssemblyFile { get; set; } = string.Empty;

        [DataMember]
        [CanBeNull]
        public string For { get; set; } = string.Empty;

    }
}
