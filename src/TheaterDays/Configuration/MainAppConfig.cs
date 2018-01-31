using OpenMLTD.MilliSim.Configuration.Entities;

namespace OpenMLTD.TheaterDays.Configuration {
    public sealed class MainAppConfig : ConfigBase {

        public MainAppConfigData Data { get; set; }

        public sealed class MainAppConfigData {

            public WindowConfig Window { get; set; }

            public sealed class WindowConfig {

                public int Width { get; set; }

                public int Height { get; set; }

                public string Title { get; set; }

            }

        }

    }
}
