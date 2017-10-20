using System;
using System.IO;
using System.Windows.Forms;
using OpenMLTD.MilliSim.Theater.Configuration.Yaml;
using OpenMLTD.MilliSim.Theater.Forms;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenMLTD.MilliSim.Theater {
    internal static class Program {

        internal static ApplicationSettings Settings { get; private set; }

        internal static PluginManager PluginManager { get; private set; }

        [STAThread]
        private static void Main(string[] args) {
            try {
                Application.EnableVisualStyles();

                if (!File.Exists(ConfigFilePath)) {
                    MessageBox.Show($"Missing config file at '{ConfigFilePath}'.", ApplicationHelper.GetTitle(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                var b = new DeserializerBuilder()
                    .WithNamingConvention(new UnderscoredNamingConvention())
                    .IgnoreUnmatchedProperties()
                    .WithTypeConverter(new PercentOrRealValueConverter())
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
                PluginManager = new PluginManager(extensionPaths);

                using (var theaterDays = new TheaterDays()) {
                    theaterDays.Run<TheaterView>(args);
                }

                PluginManager.Dispose();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static readonly string ConfigFilePath = "appconfig.yml";

    }
}
