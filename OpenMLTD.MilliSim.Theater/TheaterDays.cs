using System.IO;
using OpenMLTD.MilliSim.Audio;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Theater.Configuration.Primitives;
using OpenMLTD.MilliSim.Theater.Elements.Logical;
using OpenMLTD.MilliSim.Theater.Elements.Visual;
using OpenMLTD.MilliSim.Theater.Elements.Visual.Background;
using OpenMLTD.MilliSim.Theater.Elements.Visual.Gaming;
using OpenMLTD.MilliSim.Theater.Elements.Visual.Overlays;
using OpenMLTD.MilliSim.Theater.Elements.Visual.Overlays.Combo;
using OpenMLTD.MilliSim.Theater.Internal;

namespace OpenMLTD.MilliSim.Theater {
    public sealed class TheaterDays : VisualGame {

        public TheaterDays() {
            LibraryPreloader.PreloadLibrary("D3DCompiler_47.dll");
        }

        ~TheaterDays() {
            LibraryPreloader.UnloadAllPreloadedLibraries();
        }

        public override string Title => "THE iDOLM@STER Million Live! Theater Days Simulator";

        public AudioManager AudioManager => (AudioManager)BaseAudioManager;

        protected override AudioManagerBase CreateAudioManager() {
            return new AudioManager();
        }

        protected override void CreateElements() {
            var settings = Program.Settings;
            var stage = Stage;

            // Background elements
            // Beware the adding order. Usually these elements should be updated first.
            ComponentFactory.CreateAndAdd<SyncTimer>(stage);
            ComponentFactory.CreateAndAdd<AudioController>(stage);
            ComponentFactory.CreateAndAdd<ScoreLoader>(stage);

            // Background
            if (!string.IsNullOrEmpty(settings.Media.BackgroundAnimation) && File.Exists(settings.Media.BackgroundAnimation)) {
                ComponentFactory.CreateAndAdd<BackgroundVideo>(stage);
            } else if (!string.IsNullOrEmpty(settings.Media.BackgroundImage) && File.Exists(settings.Media.BackgroundImage)) {
                ComponentFactory.CreateAndAdd<BackgroundImage>(stage);
            }

#if DEBUG
            //elements.Add(new MiniCube(this));
#endif

            // ** Stage ** //
            {
                var gamingArea = ComponentFactory.CreateAndAdd<GamingArea>(stage);

                ComponentFactory.CreateAndAdd<NoteReactor>(gamingArea);

                if (settings.Style.SlideMotionPosition == SlideMotionPosition.Below) {
                    ComponentFactory.CreateAndAdd<SlideMotion>(gamingArea);
                }
                ComponentFactory.CreateAndAdd<RibbonsLayer>(gamingArea);
                if (settings.Style.SlideMotionPosition == SlideMotionPosition.Above) {
                    ComponentFactory.CreateAndAdd<SlideMotion>(gamingArea);
                }
                ComponentFactory.CreateAndAdd<TapPointsMergingAnimation>(gamingArea);
                var notesLayer = ComponentFactory.CreateAndAdd<NotesLayer>(gamingArea);
                notesLayer.GlobalSpeedScale = 1.3f;
                ComponentFactory.CreateAndAdd<TapPoints>(gamingArea);
                ComponentFactory.CreateAndAdd<HitRankAnimation>(gamingArea);
            }

            // Overlays

            {
                var comboDisplay = ComponentFactory.CreateAndAdd<ComboDisplay>(stage);

                ComponentFactory.CreateAndAdd<ComboAura>(comboDisplay);
                ComponentFactory.CreateAndAdd<ComboText>(comboDisplay);
                ComponentFactory.CreateAndAdd<ComboNumbers>(comboDisplay);
            }

            ComponentFactory.CreateAndAdd<AvatarDisplay>(stage);
            ComponentFactory.CreateAndAdd<SongTitle>(stage);
            ComponentFactory.CreateAndAdd<CuteIdol>(stage);

            var helpOverlay = ComponentFactory.CreateAndAdd<HelpOverlay>(stage);
            helpOverlay.Text = settings.LocalStrings.PressSpaceToStart;
            helpOverlay.Visible = false;

            if (settings.SystemUI.FpsOverlay.Use) {
                var fpsOverlay = ComponentFactory.CreateAndAdd<FpsOverlay>(stage);
                fpsOverlay.FillColor = settings.SystemUI.FpsOverlay.TextFill;
                fpsOverlay.FontSize = settings.SystemUI.FpsOverlay.FontSize;
            }

            if (settings.SystemUI.DebugOverlay.Use) {
                var debugOverlay = ComponentFactory.CreateAndAdd<DebugOverlay>(stage);
                debugOverlay.FillColor = settings.SystemUI.DebugOverlay.TextFill;
                debugOverlay.FontSize = settings.SystemUI.DebugOverlay.FontSize;
            }

            if (settings.SystemUI.SyncTimerOverlay.Use) {
                var syncTimerOverlay = ComponentFactory.CreateAndAdd<SyncTimerOverlay>(stage);
                syncTimerOverlay.FillColor = settings.SystemUI.SyncTimerOverlay.TextFill;
                syncTimerOverlay.FontSize = settings.SystemUI.SyncTimerOverlay.FontSize;
            }
        }

    }
}
