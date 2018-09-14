using OpenMLTD.MilliSim.Configuration.Entities;
using OpenMLTD.MilliSim.Configuration.Primitives;
using OpenMLTD.TheaterDays.Configuration.Primitives;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration {
    public sealed class NotesLayerConfig : ConfigBase {

        public NotesLayerConfigData Data { get; set; }

        public sealed class NotesLayerConfigData {

            public LayoutValue2D Layout { get; set; }

            public PercentOrRealValue Opacity { get; set; }

            public NotesLayerImages Images { get; set; }

            public NotesLayerStyle Style { get; set; }

            public SimulationParams Simulation { get; set; }

        }

        public sealed class NotesLayerImages {

            public ImageStrip[] Notes { get; set; }

            public ImageWithBlankEdge SpecialNote { get; set; }

            public ImageWithBlankEdge SyncLine { get; set; }

            public ImageWithBlankEdge SpecialNoteAura { get; set; }

            public ImageWithBlankEdge SpecialNoteSocket { get; set; }

        }

        public sealed class NotesLayerStyle {

            public bool SyncLine { get; set; }

            public bool SlideMiddleSyncLine { get; set; }

            [YamlMember(Alias = "trace_plugin_id")]
            public string TracePluginID { get; set; }

        }

        public sealed class SimulationParams {

            public PercentOrRealValue GlobalSpeed { get; set; }

        }

    }
}
