using OpenMLTD.MilliSim.Configuration.Entities;
using OpenMLTD.MilliSim.Configuration.Primitives;

namespace OpenMLTD.MilliSim.Theater.Configuration {
    public sealed class MainAppConfig : ConfigBase {

        public MainAppConfigData Data { get; set; }

        public sealed class MainAppConfigData {

            public SimpleSize Window { get; set; }

            public PluginsConfig Plugins { get; set; }

        }

        public sealed class PluginsConfig {

            public PluginsLoading Loading { get; set; }

            public string[] ComponentFactories { get; set; }

        }

        public sealed class PluginsLoading {

            public PluginsLoadingMode Mode { get; set; }

            public PluginsLoadingLists Lists { get; set; }

            public sealed class PluginsLoadingLists {

                public string[] WhiteList { get; set; }

                public string[] BlackList { get; set; }

            }

        }

    }
}
