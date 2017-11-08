using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Configuration.Entities {
    public sealed class ConfigMetadata {

        public int Version { get; set; }

        [NotNull]
        public string Type { get; set; }

        [NotNull]
        public string AssemblyFile { get; set; }

        public string For { get; set; }

    }
}
