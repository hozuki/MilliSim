using System;
using System.IO;
using System.Windows.Forms;
using OpenMLTD.MilliSim.Theater.Configuration.Yaml;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenMLTD.MilliSim.Theater {
    internal static class Program {

        public static ApplicationSettings Settings { get; private set; }

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
                .WithTypeConverter(new SizeFConverter());
            var s = b.Build();

            using (var fileStream = File.Open(ConfigFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var reader = new StreamReader(fileStream)) {
                    var settings = s.Deserialize<ApplicationSettings>(reader);
                    Settings = settings;
                }
            }

            using (var theaterDays = new TheaterDays()) {
                theaterDays.Run<TheaterView>(args);
            }
        }

        private static readonly string ConfigFilePath = "appconfig.yml";

    }
}
