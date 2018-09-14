using System;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.TheaterDays.Configuration.Extending;
using YamlDotNet.Serialization;

namespace OpenMLTD.TheaterDays.Configuration.Converters {
    [MilliSimPlugin(typeof(IConfigTypeConverterFactory))]
    public sealed class PercentOrRealValueConverterFactory : ConfigTypeConverterFactory {

        public override string PluginID => "plugin.configuration.converter_factory.percent_or_real_value";

        public override string PluginName => "Percent-or-Real-Value Converter Factory";

        public override string PluginDescription => "Converts a percent number or literal floating point number from strings";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IYamlTypeConverter CreateTypeConverter() {
            return new PercentOrRealValueConverter();
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
