using System;
using OpenMLTD.TheaterDays.Configuration.Extending;
using YamlDotNet.Serialization;

namespace OpenMLTD.TheaterDays.Configuration {
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
