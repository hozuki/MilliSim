using OpenMLTD.MilliSim.Core;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Configuration.Extending {
    /// <summary>
    /// A factory that creates type converters to processs YAML entities.
    /// This interface will be called before the initialization of other plugins.
    /// </summary>
    public interface IConfigTypeConverterFactory : IMilliSimPlugin {

        /// <summary>
        /// Create a new type converter.
        /// </summary>
        /// <returns></returns>
        IYamlTypeConverter CreateTypeConverter();

    }
}
