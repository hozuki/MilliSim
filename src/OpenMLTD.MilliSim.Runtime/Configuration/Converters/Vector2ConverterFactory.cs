using System;
using OpenMLTD.MilliSim.Configuration.Extending;
using OpenMLTD.MilliSim.Core;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Configuration.Converters {
    [MilliSimPlugin(typeof(IConfigTypeConverterFactory))]
    public sealed class Vector2ConverterFactory : ConfigTypeConverterFactory {

        public override string PluginID => "plugin.configuration.converter_factory.vector2";

        public override string PluginName => "Vector2 Converter Factory";

        public override string PluginDescription => "Converts XNA 2D vectors from strings";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IYamlTypeConverter CreateTypeConverter() {
            return new Vector2Converter();
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
