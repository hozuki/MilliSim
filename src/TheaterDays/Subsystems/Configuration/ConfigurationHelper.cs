using JetBrains.Annotations;
using OpenMLTD.MilliSim.Configuration;
using OpenMLTD.MilliSim.Configuration.Converters;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Plugin;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenMLTD.TheaterDays.Subsystems.Configuration {
    internal static class ConfigurationHelper {

        internal static ConfigurationStore CreateConfigurationStore([NotNull] BasePluginManager pluginManager) {
            var typeConverterFactories = pluginManager.GetPluginsOfType<IConfigTypeConverterFactory>();

            var deserializerBuilder = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(new UnderscoredNamingConvention())
                // Internal converters
                .WithTypeConverter(new AdvancedEnumConverter(new PascalCaseNamingConvention()))
                .WithTypeConverter(new PercentOrRealValueConverter())
                .WithTypeConverter(new ColorConverter())
                .WithTypeConverter(new PointFConverter())
                .WithTypeConverter(new SizeFConverter());

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
