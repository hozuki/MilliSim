using OpenMLTD.MilliSim.Theater.Configuration.Primitives;

namespace OpenMLTD.MilliSim.Theater.Configuration {
    public sealed class ImagesClass {

        public ImageWithBlankEdge TapPoint { get; set; }

        public ImageWithBlankEdge TapBarChain { get; set; }

        public ImageWithBlankEdge TapBarNode { get; set; }

        public HpGaugeClass HpGauge { get; set; }

        public ImageWithBlankEdge[] Avatars { get; set; }

        public ScoreRankClass ScoreRank { get; set; }

        public ImageWithBlankEdge ComboCounter { get; set; }

        public ImageStrip FullCombo { get; set; }

        public ImageStrip LiveClear { get; set; }

        public ImageWithBlankEdge ScoreNumbers { get; set; }

        public ImageWithBlankEdge ComboNumbers { get; set; }

        public ImageStrip HitRank { get; set; }

        public ImageStrip[] Notes { get; set; }

        public ImageWithBlankEdge SpecialNote { get; set; }

        public ImageWithBlankEdge Ribbon { get; set; }

        public ImageWithBlankEdge SyncLine { get; set; }

        public ImageStrip DifficultyBadges { get; set; }

        public ImageWithBlankEdge SpecialNoteAura { get; set; }

        public ImageWithBlankEdge SpecialNoteSocket { get; set; }

        public sealed class HpGaugeClass {

            public ImageWithBlankEdge Base { get; set; }

            public ImageWithBlankEdge Progress { get; set; }

        }

        public sealed class ScoreRankClass {

            public ImageWithBlankEdge Base { get; set; }

            public ImageWithBlankEdge Icons { get; set; }

        }

    }
}
