using CommandLine;

namespace OpenMLTD.TheaterDays {
    public sealed class Options {

        [Option("debug", HelpText = "Enable debug mode", Required = false, Default = false)]
        public bool IsDebugEnabled { get; set; }

        [Option("editor_port", HelpText = "BVSP editor server port", Required = false, Default = -1)]
        public int EditorServerPort { get; set; }

#if DEBUG
        [Option("bvsp_port", HelpText = "BVSP port", Required = false, Default = -1)]
        public int BvspPort { get; set; }
#endif

    }
}
