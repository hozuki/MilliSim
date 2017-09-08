using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core.Entities.Extending;
using OpenMLTD.MilliSim.Theater.Configuration.Yaml;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenMLTD.MilliSim.Theater {
    internal static class Program {

        public static ApplicationSettings Settings { get; private set; }

        [ImportMany]
        public static IReadOnlyList<IScoreReader> ScoreReaders { get; [UsedImplicitly] private set; }

        internal static CompositionHost ExtensionContainer { get; private set; }

        [STAThread]
        private static void Main(string[] args) {
            Application.EnableVisualStyles();

            if (!File.Exists(ConfigFilePath)) {
                MessageBox.Show($"Missing config file at '{ConfigFilePath}'.", ApplicationHelper.GetTitle(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var b = new DeserializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .IgnoreUnmatchedProperties()
                .WithTypeConverter(new PercentOrRealValueConverter())
                .WithTypeConverter(new DifficultyConverter())
                .WithTypeConverter(new ColorConverter())
                .WithTypeConverter(new SizeFConverter())
                .WithTypeConverter(new PointFConverter());
            var s = b.Build();

            using (var fileStream = File.Open(ConfigFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var reader = new StreamReader(fileStream)) {
                    var settings = s.Deserialize<ApplicationSettings>(reader);
                    Settings = settings;
                }
            }

            var extensionPaths = new[] {
                Environment.CurrentDirectory,
                Path.Combine(Environment.CurrentDirectory, "plugins")
            };
            ExtensionContainer = LoadExtensionsFromDirectories(extensionPaths);

            using (var theaterDays = new TheaterDays()) {
                theaterDays.Run<TheaterView>(args);
            }

            ExtensionContainer.Dispose();
        }

        private static CompositionHost LoadExtensionsFromDirectories(params string[] directories) {
            var allAssemblies = new List<Assembly>();
            foreach (var directory in directories) {
                if (!Directory.Exists(directory)) {
                    continue;
                }

                var assemblyFileNames = Directory
                    .EnumerateFiles(directory)
                    .Where(str => str.ToLowerInvariant().EndsWith(".dll"));

                foreach (var assemblyFileName in assemblyFileNames) {
                    try {
                        var assembly = Assembly.LoadFrom(assemblyFileName);
                        allAssemblies.Add(assembly);
                    } catch (Exception ex) {
                        Debug.Print(ex.Message);
                    }
                }
            }

            var configuration = new ContainerConfiguration().WithAssemblies(allAssemblies);
            var host = configuration.CreateContainer();

            ScoreReaders = host.GetExports<IScoreReader>().ToArray();

            return host;
        }

        private static readonly string ConfigFilePath = "appconfig.yml";

    }
}
