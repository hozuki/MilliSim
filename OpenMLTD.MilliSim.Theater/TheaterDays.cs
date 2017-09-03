using System.Collections.Generic;
using System.IO;
using OpenMLTD.MilliSim.Audio;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Theater.Elements;

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
            elements.Add(new AudioController(this));

            // Background
            if (!string.IsNullOrEmpty(settings.Media.BackgroundAnimation) && File.Exists(settings.Media.BackgroundAnimation)) {
                elements.Add(new BackgroundVideo(this));
            } else if (!string.IsNullOrEmpty(settings.Media.BackgroundImage) && File.Exists(settings.Media.BackgroundImage)) {
                elements.Add(new BackgroundImage(this));
            }

#if DEBUG
            //elements.Add(new MiniCube(this));
#endif
            elements.Add(new RibbonsLayer(this));


            // ** Stage ** //

            var gamingAreaElements = new List<Element>();

            gamingAreaElements.Add(new NotesLayer(this));
            gamingAreaElements.Add(new TapPoints(this));

            var stage = new GamingArea(this, gamingAreaElements.ToArray());
            elements.Add(stage);

            // Overlays

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

            return elements.ToArray();
        }

    }
}
