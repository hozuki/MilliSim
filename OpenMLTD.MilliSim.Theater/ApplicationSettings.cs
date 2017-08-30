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

        public LayoutClass Layout { get; set; }

        public StyleClass Style { get; set; }

        public GameClass Game { get; set; }

        [YamlMember(Alias = "system_ui")]
        public SystemUIClass SystemUI { get; set; }

        public SfxClass Sfx { get; set; }

        [YamlMember(Alias = "localization")]
        public LocalStringsClass LocalStrings { get; set; }

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

            public string TapPoints { get; set; }

            public HpGaugeClass HpGauge { get; set; }

            public AvatarsClass Avatars { get; set; }

            public ScoreRankClass ScoreRank { get; set; }

            public string ComboCounter { get; set; }

            public ImageStrip FullCombo { get; set; }

            public ImageStrip LiveClear { get; set; }

            public string ScoreNumbers { get; set; }

            public string ComboNumbers { get; set; }

            public ImageStrip HitRank { get; set; }

            public string[] Notes { get; set; }

            public string SyncLine { get; set; }

            public ImageStrip DifficultyBadges { get; set; }

            public sealed class HpGaugeClass {

                public string Base { get; set; }

                public string Progress { get; set; }

            }

            public sealed class AvatarsClass {

                public string Mask { get; set; }

                public string[] Icons { get; set; }

            }

            public sealed class ScoreRankClass {

                public string Base { get; set; }

                public string Icons { get; set; }

            }

        }

        public sealed class LayoutClass {

            public LayoutCoordinate2D TapPoints { get; set; }

            public LayoutCoordinate2D HpGauge { get; set; }

            public LayoutCoordinate2D Avatars { get; set; }

            public LayoutCoordinate2D ScoreRank { get; set; }

            public LayoutCoordinate2D ComboCounter { get; set; }

            public NoteAreaClass NoteArea { get; set; }

            public LayoutCoordinate2D Title { get; set; }

            public LayoutCoordinate2D HitRank { get; set; }

            public sealed class NoteAreaClass {

                public LayoutValue Top { get; set; }

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

        public sealed class ImageStrip {

            public string File { get; set; }

            public int Count { get; set; }

        }

        public sealed class LayoutCoordinate2D {

            public LayoutValue X { get; set; }

            public LayoutValue Y { get; set; }

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

    }
}
