using CommandLine;

namespace OpenMLTD.TheaterDays {
    public sealed class Options {

        [Option("debug", HelpText = "Enable debug mode", Required = false, Default = false)]
        public bool IsDebugEnabled { get; set; }

        [Option("editor_port", HelpText = "BVSP editor server port", Required = false, Default = -1)]
        public int EditorServerPort { get; set; }

        [Option("editor_server_uri", HelpText = "BVSP editor server URI, overrides editor_port", Required = false, Default = null)]
        public string EditorServerUri { get; set; }

#if DEBUG
        [Option("bvsp_port", HelpText = "BVSP simulator server port override", Required = false, Default = -1)]
        public int BvspPort { get; set; }
#endif

    }
}
