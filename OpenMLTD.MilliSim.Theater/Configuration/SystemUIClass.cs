using OpenMLTD.MilliSim.Theater.Configuration.Primitives;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Theater.Configuration {
    public sealed class SystemUIClass {

        public UIElementConfig DebugOverlay { get; set; }

        public UIElementConfig FpsOverlay { get; set; }

        [YamlMember(Alias = "timer_overlay")]
        public UIElementConfig SyncTimerOverlay { get; set; }

    }
}
