using System.Collections.Generic;
using System.IO;
using OpenMLTD.MilliSim.Audio;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Theater.Configuration.Primitives;
using OpenMLTD.MilliSim.Theater.Elements.Logical;
using OpenMLTD.MilliSim.Theater.Elements.Visual.Background;
using OpenMLTD.MilliSim.Theater.Elements.Visual.Gaming;
using OpenMLTD.MilliSim.Theater.Elements.Visual.Overlays;

namespace OpenMLTD.MilliSim.Theater {
    public sealed class TheaterDays : VisualGame {

        public override string Title => "THE iDOLM@STER Million Live Theater Days Simulator";

        public AudioManager AudioManager => (AudioManager)BaseAudioManager;

        protected override AudioManagerBase CreateAudioManager() {
            return new AudioManager();
        }

        protected override IReadOnlyList<IElement> CreateElements() {
            var settings = Program.Settings;
            var elements = new List<Element>();

            // Background elements
            // Beware the adding order. Usually these elements should be updated first.
            elements.Add(new SyncTimer(this));
            elements.Add(new AudioController(this));
            elements.Add(new ScoreLoader(this));

            // Background
            if (!string.IsNullOrEmpty(settings.Media.BackgroundAnimation) && File.Exists(settings.Media.BackgroundAnimation)) {
                elements.Add(new BackgroundVideo(this));
            } else if (!string.IsNullOrEmpty(settings.Media.BackgroundImage) && File.Exists(settings.Media.BackgroundImage)) {
                elements.Add(new BackgroundImage(this));
            }

#if DEBUG
            //elements.Add(new MiniCube(this));
#endif

            // ** Stage ** //

            var gamingAreaElements = new List<Element>();

            gamingAreaElements.Add(new NoteReactor(this));
            if (settings.Style.SlideMotionPosition == SlideMotionPosition.Below) {
                gamingAreaElements.Add(new SlideMotion(this));
            }
            gamingAreaElements.Add(new RibbonsLayer(this));
            if (settings.Style.SlideMotionPosition == SlideMotionPosition.Above) {
                gamingAreaElements.Add(new SlideMotion(this));
            }
            gamingAreaElements.Add(new TapPointsMergingAnimation(this));
            gamingAreaElements.Add(new NotesLayer(this) {
                GlobalSpeedScale = 1.3f
            });
            gamingAreaElements.Add(new TapPoints(this));
            gamingAreaElements.Add(new HitRankAnimation(this));

            var stage = new GamingArea(this, gamingAreaElements.ToArray());
            elements.Add(stage);

            // Overlays

            elements.Add(new AvatarDisplay(this));

            elements.Add(new SongTitle(this));

            elements.Add(new HelpOverlay(this) {
                Text = settings.LocalStrings.PressSpaceToStart,
                Visible = false
            });

            if (settings.SystemUI.FpsOverlay.Use) {
                elements.Add(new FpsOverlay(this) {
                    FillColor = settings.SystemUI.FpsOverlay.TextFill,
                    FontSize = settings.SystemUI.FpsOverlay.FontSize
                });
            }

            if (settings.SystemUI.DebugOverlay.Use) {
                elements.Add(new DebugOverlay(this) {
                    FillColor = settings.SystemUI.DebugOverlay.TextFill,
                    FontSize = settings.SystemUI.DebugOverlay.FontSize
                });
            }

            if (settings.SystemUI.SyncTimerOverlay.Use) {
                elements.Add(new SyncTimerOverlay(this) {
                    FillColor = settings.SystemUI.SyncTimerOverlay.TextFill,
                    FontSize = settings.SystemUI.SyncTimerOverlay.FontSize
                });
            }

            return elements.ToArray();
        }

    }
}
