using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Theater.Configuration {
    public sealed class StyleClass {

        [YamlMember(Alias = "trace_plugin")]
        public string TracePluginID { get; set; }

        public bool SyncLine { get; set; }

        public bool FlickRibbon { get; set; }

        public bool SlideMiddleSyncLine { get; set; }

    }
}
