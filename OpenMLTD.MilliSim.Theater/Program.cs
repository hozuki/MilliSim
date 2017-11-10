using System;
using System.IO;
using System.Windows.Forms;
using OpenMLTD.MilliSim.GameAbstraction;
using OpenMLTD.MilliSim.Theater.Configuration;
using OpenMLTD.MilliSim.Theater.Forms;

namespace OpenMLTD.MilliSim.Theater {
    internal static class Program {

        [STAThread]
        private static void Main(string[] args) {
#if !DEBUG
            try {
#endif
            Application.EnableVisualStyles();

            if (!File.Exists(ConfigFilePath)) {
                MessageBox.Show($"Missing config file at '{ConfigFilePath}'.", ApplicationHelper.GetTitle(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var extensionPaths = new[] {
                    Environment.CurrentDirectory,
                    Path.Combine(Environment.CurrentDirectory, "plugins")
                };

            using (var theaterDays = new TheaterDays()) {
                theaterDays.LoadConfigurations();
                theaterDays.Globalize();

                var pluginsConfig = theaterDays.ConfigurationStore.Get<MainAppConfig>();
                var pluginManager = new PluginManager();
                theaterDays.PluginManager = pluginManager;

                var loadingMode = pluginsConfig.Data.Plugins.Loading.Mode;
                string[] pluginList;
                switch (loadingMode) {
                    case PluginsLoadingMode.Default:
                        pluginList = null;
                        break;
                    case PluginsLoadingMode.BlackList:
                        pluginList = pluginsConfig.Data.Plugins.Loading.Lists.BlackList;
                        break;
                    case PluginsLoadingMode.WhiteList:
                        pluginList = pluginsConfig.Data.Plugins.Loading.Lists.WhiteList;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                pluginManager.LoadAssemblies((PluginSearchingMode)loadingMode, pluginList, extensionPaths);

                theaterDays.Run<TheaterView>(args);
            }
#if !DEBUG
            } catch (Exception ex) {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
#endif
        }

        internal static readonly string ConfigFilePath = "appconfig.yml";

    }
}
