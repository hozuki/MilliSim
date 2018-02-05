using JetBrains.Annotations;
using OpenMLTD.MilliSim.Configuration;
using OpenMLTD.MilliSim.Configuration.Extending;
using OpenMLTD.MilliSim.Foundation;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenMLTD.TheaterDays.Subsystems.Configuration {
    internal static class ConfigurationHelper {

        internal static ConfigurationStore CreateConfigurationStore([NotNull] BasePluginManager pluginManager) {
            var typeConverterFactories = pluginManager.GetPluginsOfType<IConfigTypeConverterFactory>();

            var deserializerBuilder = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(new UnderscoredNamingConvention());

            // External converters
            foreach (var factory in typeConverterFactories) {
                var converter = factory.CreateTypeConverter();
                deserializerBuilder.WithTypeConverter(converter);
            }

            var deserializer = deserializerBuilder.Build();

            return ConfigurationStore.Load(ConfigurationEntryFile, deserializer);
        }

        private static readonly string ConfigurationEntryFile = "Contents/app.config.yml";

    }
}
