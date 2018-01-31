using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Plugin {
    public interface IConfigTypeConverterFactory : IMilliSimPlugin {

        IYamlTypeConverter CreateTypeConverter();

    }
}
