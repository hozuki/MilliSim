using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Configuration.Entities {
    /// <summary>
    /// Entity class for configuration items.
    /// This class must be inherited.
    /// </summary>
    [DataContract]
    public abstract class ConfigBase {

        /// <summary>
        /// Gets or sets the metadata of this item.
        /// </summary>
        [DataMember]
        [NotNull]
        public ConfigMetadata Metadata { get; set; } = new ConfigMetadata();

    }
}
