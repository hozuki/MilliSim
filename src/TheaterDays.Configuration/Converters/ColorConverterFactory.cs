using System;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.TheaterDays.Configuration.Extending;
using YamlDotNet.Serialization;

namespace OpenMLTD.TheaterDays.Configuration.Converters {
    [MilliSimPlugin(typeof(IConfigTypeConverterFactory))]
    public sealed class ColorConverterFactory : ConfigTypeConverterFactory {

        public override string PluginID => "plugin.configuration.converter_factory.color";

        public override string PluginName => "Color Converter Factory";

        public override string PluginDescription => "Converts XNA colors from strings";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IYamlTypeConverter CreateTypeConverter() {
            return new ColorConverter();
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
