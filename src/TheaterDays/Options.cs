using CommandLine;

namespace OpenMLTD.TheaterDays {
    public sealed class Options {

        [Option("debug", HelpText = "Enable debug mode", Required = false, Default = false)]
        public bool IsDebugEnabled { get; set; }

    }
}
