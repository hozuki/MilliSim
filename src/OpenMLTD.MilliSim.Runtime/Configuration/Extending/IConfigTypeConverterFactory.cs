using OpenMLTD.MilliSim.Core;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Configuration.Extending {
    public interface IConfigTypeConverterFactory : IMilliSimPlugin {

        IYamlTypeConverter CreateTypeConverter();

    }
}
