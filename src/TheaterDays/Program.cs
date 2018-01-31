using CommandLine;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.TheaterDays.Subsystems.Configuration;
using OpenMLTD.TheaterDays.Subsystems.Globalization;
using OpenMLTD.TheaterDays.Subsystems.Plugin;

namespace OpenMLTD.TheaterDays {
    internal static class Program {

        private static int Main([NotNull, ItemNotNull] string[] args) {
#if ENABLE_GUI_CONSOLE
            GuiConsole.Initialize();
            GuiConsole.Error.WriteLine();
#endif

            var optionsParsingResult = Parser.Default.ParseArguments<Options>(args);
            int exitCode;

            if (optionsParsingResult.Tag == ParserResultType.Parsed) {
                var options = ((Parsed<Options>)optionsParsingResult).Value;

                GameLog.Initialize("theater-days");
                GameLog.Enabled = options.IsDebugEnabled;

                using (var pluginManager = new TheaterDaysPluginManager()) {
                    pluginManager.LoadPlugins();

                    var configurationStore = ConfigurationHelper.CreateConfigurationStore(pluginManager);
                    var cultureSpecificInfo = CultureSpecificInfoHelper.CreateCultureSpecificInfo();

                    using (var game = new Theater(pluginManager, configurationStore, cultureSpecificInfo)) {
                        game.Run();
                    }
                }

                exitCode = 0;
            } else {
                var helpText = CommandLine.Text.HelpText.AutoBuild(optionsParsingResult);

                // TODO: Does this even work?
                GameLog.Info(helpText);

                exitCode = -1;
            }

#if ENABLE_GUI_CONSOLE
            GuiConsole.Uninitialize();
#endif

            return exitCode;
        }

    }
}
