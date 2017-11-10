using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Glob;
using OpenMLTD.MilliSim.Audio;
using OpenMLTD.MilliSim.Configuration;
using OpenMLTD.MilliSim.Configuration.Converters;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.GameAbstraction;
using OpenMLTD.MilliSim.Theater.Configuration;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using OpenMLTD.MilliSim.Globalization;

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

        protected override CultureSpecificInfo CreateCultureSpecificInfo() {
            var cultureSpecificInfo = new CultureSpecificInfo(CultureInfo.CurrentUICulture);

            // Load translation files.
            var tm = cultureSpecificInfo.TranslationManager;
            var config = ConfigurationStore.Get<MainAppConfig>();
            var paths = config.Data.TranslationFiles;

            foreach (var path in paths) {
                var fullPath = Path.IsPathRooted(path) ? path : Path.Combine(Environment.CurrentDirectory, path);

                var globCharIndex = fullPath.IndexOfAny(PartialGlobChars);

                if (globCharIndex == 0) {
                    throw new ArgumentException(nameof(globCharIndex));
                }

                var finalIndex = globCharIndex > 0 ? globCharIndex : fullPath.Length;
                var lastPathSeparatorIndex = fullPath.LastIndexOfAny(PathSeparators, finalIndex);
                // Include the last separator
                var directoryName = fullPath.Substring(0, lastPathSeparatorIndex + 1);

                var baseDirectory = new DirectoryInfo(directoryName);
                var pattern = fullPath.Substring(directoryName.Length);

                foreach (var fileInfo in baseDirectory.GlobFiles(pattern)) {
                    tm.AddTranslationsFromFile(fileInfo.FullName);
                }
            }

            return cultureSpecificInfo;
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

        private static readonly char[] PartialGlobChars = { '*', '?' };
        private static readonly char[] PathSeparators = { '\\', '/' };

    }
}
