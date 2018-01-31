using JetBrains.Annotations;
using OpenMLTD.TheaterDays.Subsystems.Plugin;

namespace OpenMLTD.TheaterDays.Configuration {
    public sealed class PluginsConfig {

        public PluginsLoading Loading { get; set; }

        public string[] ComponentFactories { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
        public sealed class PluginsLoading {

            public PluginSearchMode Mode { get; set; }

            public PluginsLoadingLists Lists { get; set; }

            [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
            public sealed class PluginsLoadingLists {

                public string[] WhiteList { get; set; }

                public string[] BlackList { get; set; }

            }

        }

    }
}
