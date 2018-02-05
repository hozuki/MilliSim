using CommandLine;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.TheaterDays.Subsystems.Configuration;
using OpenMLTD.TheaterDays.Subsystems.Globalization;
using OpenMLTD.TheaterDays.Subsystems.Plugin;

namespace OpenMLTD.TheaterDays {
    partial class Theater {

        public static int Run([NotNull, ItemNotNull] string[] args, GraphicsBackend graphicsBackend, [NotNull] string loggerName = "theater-days") {
            GraphicsBackend = graphicsBackend;

            GameLog.Initialize(loggerName);
            GameLog.Enabled = true;

            var optionsParsingResult = Parser.Default.ParseArguments<Options>(args);
            int exitCode;

#if ENABLE_GUI_CONSOLE
            GuiConsole.Initialize();
            GuiConsole.Error.WriteLine();
#endif
            if (optionsParsingResult.Tag == ParserResultType.Parsed) {
                var options = ((Parsed<Options>)optionsParsingResult).Value;

                GameLog.Enabled = options.IsDebugEnabled;

                using (var pluginManager = new TheaterDaysPluginManager()) {
                    pluginManager.LoadPlugins();

                    var configurationStore = ConfigurationHelper.CreateConfigurationStore(pluginManager);
                    var cultureSpecificInfo = CultureSpecificInfoHelper.CreateCultureSpecificInfo();

                    using (var game = new Theater(pluginManager, configurationStore, cultureSpecificInfo)) {
                        game.Run();
                    }

                    exitCode = 0;
                }
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
