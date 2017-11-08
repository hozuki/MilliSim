using System.Diagnostics;
using System.Linq;
using OpenMLTD.MilliSim.Audio;
using OpenMLTD.MilliSim.Configuration;
using OpenMLTD.MilliSim.Configuration.Converters;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.GameAbstraction;
using OpenMLTD.MilliSim.Theater.Configuration;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenMLTD.MilliSim.Theater {
    public sealed class TheaterDays : TheaterDaysBase {

        public TheaterDays() {
            LibraryPreloader.PreloadLibrary("D3DCompiler_47.dll");
        }

        ~TheaterDays() {
            LibraryPreloader.UnloadAllPreloadedLibraries();
        }

        public override string Title => "THE iDOLM@STER Million Live! Theater Days Simulator";

        protected override AudioManagerBase CreateAudioManager() {
            return new AudioManager();
        }

        protected override ConfigurationStore CreateConfigurationStore() {
            var desBuilder = new DeserializerBuilder();
            var deserializer = desBuilder
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .WithTypeConverter(new AdvancedEnumConverter(new PascalCaseNamingConvention()))
                .WithTypeConverter(new PercentOrRealValueConverter())
                .WithTypeConverter(new ColorConverter())
                .WithTypeConverter(new PointFConverter())
                .WithTypeConverter(new SizeFConverter())
                .Build();
            var store = ConfigurationStore.Load(Program.ConfigFilePath, deserializer);
            return store;
        }

        protected override void CreateComponents() {
            var config = ConfigurationStore.Get<MainAppConfig>();
            var stage = Stage;
            var factories = PluginManager.GetPluginsOfType<IComponentFactory>();

            foreach (var factoryID in config.Data.Plugins.ComponentFactories) {
                var factory = factories.SingleOrDefault(f => f.PluginID == factoryID);
                if (factory == null) {
                    Debug.Print("Warning: cannot find component factory '{0}'.", factoryID);
                    continue;
                }

                var component = factory.CreateComponent(this, stage);
                if (component != null) {
                    stage.Components.Add(component);
                }
            }
        }

    }
}
