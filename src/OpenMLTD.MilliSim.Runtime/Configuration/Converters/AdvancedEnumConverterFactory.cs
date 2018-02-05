using System;
using OpenMLTD.MilliSim.Configuration.Extending;
using OpenMLTD.MilliSim.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenMLTD.MilliSim.Configuration.Converters {
    [MilliSimPlugin(typeof(IConfigTypeConverterFactory))]
    public sealed class AdvancedEnumConverterFactory : ConfigTypeConverterFactory {

        public override string PluginID => "plugin.configuration.converter_factory.advanced_enum";

        public override string PluginName => "Advanced Enum Converter Factory";

        public override string PluginDescription => "Converts enums from strings";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IYamlTypeConverter CreateTypeConverter() {
            return new AdvancedEnumConverter(new PascalCaseNamingConvention());
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
