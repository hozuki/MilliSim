using System;
using OpenMLTD.MilliSim.Plugin;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Configuration {
    public abstract class ConfigTypeConverterFactory : IConfigTypeConverterFactory {

        public int ApiVersion => 1;

        public abstract string PluginID { get; }

        public abstract string PluginName { get; }

        public abstract string PluginDescription { get; }

        public abstract string PluginAuthor { get; }

        public abstract Version PluginVersion { get; }

        public string PluginCategory => "Config Type Converter Factory";

        public abstract IYamlTypeConverter CreateTypeConverter();

    }
}
