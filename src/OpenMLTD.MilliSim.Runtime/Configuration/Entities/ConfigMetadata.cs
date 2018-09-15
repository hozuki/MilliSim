using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Configuration.Entities {
    /// <summary>
    /// Metadata entity for configuration items.
    /// </summary>
    public sealed class ConfigMetadata {

        public int Version { get; set; }

        [NotNull]
        public string Type { get; set; } = string.Empty;

        [NotNull]
        public string AssemblyFile { get; set; } = string.Empty;

        [CanBeNull]
        public string For { get; set; } = string.Empty;

    }
}
