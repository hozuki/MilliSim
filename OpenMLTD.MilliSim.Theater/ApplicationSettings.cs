using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Theater.Configuration;
using System.Drawing;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Theater {
    public sealed class ApplicationSettings {

        public MetadataClass Metadata { get; set; }

        public WindowClass Window { get; set; }

        public ImagesClass Images { get; set; }

        public MediaClass Media { get; set; }

        [YamlMember(Alias = "ui")]
        public UIClass UI { get; set; }

        public StyleClass Style { get; set; }

        public GameClass Game { get; set; }

        [YamlMember(Alias = "system_ui")]
        public SystemUIClass SystemUI { get; set; }

        public SfxClass Sfx { get; set; }

        [YamlMember(Alias = "localization")]
        public LocalStringsClass LocalStrings { get; set; }

        public ScalingClass Scaling { get; set; }

        #region Setting types
        public sealed class MetadataClass {

            public int Version { get; set; }

        }

        public sealed class WindowClass {

            public WindowOrientation Orientation { get; set; }

            public int Width { get; set; }

            public int Height { get; set; }

            public FontsClass Fonts { get; set; }

            public sealed class FontsClass {

                [YamlMember(Alias = "ui")]
                public string UI { get; set; }

                public string SongTitle { get; set; }

            }

        }

        public sealed class ImagesClass {

            public ImageWithBlankEdge TapPoint { get; set; }

            public ImageWithBlankEdge TapBarChain { get; set; }

            public ImageWithBlankEdge TapBarNode { get; set; }

            public HpGaugeClass HpGauge { get; set; }

            public AvatarsClass Avatars { get; set; }

            public ScoreRankClass ScoreRank { get; set; }

            public ImageWithBlankEdge ComboCounter { get; set; }

            public ImageStrip FullCombo { get; set; }

            public ImageStrip LiveClear { get; set; }

            public ImageWithBlankEdge ScoreNumbers { get; set; }

            public ImageWithBlankEdge ComboNumbers { get; set; }

            public ImageStrip HitRank { get; set; }

            public ImageWithBlankEdge[] Notes { get; set; }

            public ImageWithBlankEdge SyncLine { get; set; }

            public ImageStrip DifficultyBadges { get; set; }

            public sealed class HpGaugeClass {

                public ImageWithBlankEdge Base { get; set; }

                public ImageWithBlankEdge Progress { get; set; }

            }

            public sealed class AvatarsClass {

                public ImageWithBlankEdge Mask { get; set; }

                public ImageWithBlankEdge[] Icons { get; set; }

            }

            public sealed class ScoreRankClass {

                public ImageWithBlankEdge Base { get; set; }

                public ImageWithBlankEdge Icons { get; set; }

            }

        }

        public sealed class UIClass {

            public UICommonConfig TapPoints { get; set; }

            public UICommonConfig HpGauge { get; set; }

            public UICommonConfig Avatars { get; set; }

            public UICommonConfig ScoreRank { get; set; }

            public UICommonConfig ComboCounter { get; set; }

            public UICommonConfig NoteArea { get; set; }

            public UICommonConfig Title { get; set; }

            public UICommonConfig HitRank { get; set; }

            public sealed class UICommonConfig {

                public LayoutCoordinate2D Layout { get; set; }

                public PercentOrRealValue Opacity { get; set; }

            }

        }

        public sealed class MediaClass {

            [YamlMember(Alias = "bgm")]
            public string BackgroundMusic { get; set; }

            [YamlMember(Alias = "bga")]
            public string BackgroundAnimation { get; set; }

            [YamlMember(Alias = "bgi")]
            public string BackgroundImage { get; set; }

            [YamlMember(Alias = "bgm_volume")]
            public int BackgroundMusicVolume { get; set; }

            [YamlMember(Alias = "bga_volume")]
            public int BackgroundAnimationVolume { get; set; }

            [YamlMember(Alias = "sfx_volume")]
            public int SoundEffectsVolume { get; set; }

        }

        public sealed class StyleClass {

            public bool SyncLine { get; set; }

            public bool FlickRibbon { get; set; }

        }

        public sealed class GameClass {

            public Difficulty Difficulty { get; set; }

            public string Title { get; set; }

            [YamlMember(Alias = "scoreobj")]
            public string ScoreFile { get; set; }

        }

        public sealed class SystemUIClass {

            public UIElementConfig DebugOverlay { get; set; }

            public UIElementConfig FpsOverlay { get; set; }

        }

        public sealed class SfxClass {

            public NoteGroup Tap { get; set; }

            public NoteGroup Hold { get; set; }

            public NoteGroup Flick { get; set; }

            public NoteGroup Slide { get; set; }

        }

        public sealed class LocalStringsClass {

            public string PressSpaceToStart { get; set; }

        }

        public sealed class ScalingClass {

            public SizeF Base { get; set; }

            public SizeF TapPoint { get; set; }

            public SizeF TapBarChain { get; set; }

            public SizeF TapBarNode { get; set; }

        }
        #endregion

        #region Setting base types
        public sealed class ImageStrip {

            public string File { get; set; }

            public int Count { get; set; }

        }

        public struct LayoutCoordinate2D {

            public PercentOrRealValue X { get; set; }

            public PercentOrRealValue Y { get; set; }

        }

        public sealed class NoteGroup {

            public string Perfect { get; set; }

            public string Great { get; set; }

            public string Nice { get; set; }

            public string Bad { get; set; }

            public string Miss { get; set; }

        }

        public sealed class UIElementConfig {

            public bool Use { get; set; }

            public float FontSize { get; set; }

            public Color TextFill { get; set; }

            public Color TextStroke { get; set; }

        }

        public sealed class ImageWithBlankEdge {

            [YamlMember(Alias = "file")]
            public string FileName { get; set; }

            public SimpleRectangleF BlankEdge { get; set; }

        }

        public struct SimpleRectangleF {

            public float Left { get; set; }

            public float Top { get; set; }

            public float Right { get; set; }

            public float Bottom { get; set; }

        }
        #endregion

    }
}
