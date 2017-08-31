using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Rendering;
using OpenMLTD.MilliSim.Theater.Elements;

namespace OpenMLTD.MilliSim.Theater {
    public sealed class TheaterDays : Game {

        public TheaterDays()
            : base(CreateElements()) {
        }

        public override string Title => "THE iDOLM@STER Million Live Theater Days Simulator";

        [NotNull, ItemNotNull]
        private static IReadOnlyList<Element> CreateElements() {
            var settings = Program.Settings;
            var elements = new List<Element>();

            if (!string.IsNullOrEmpty(settings.Media.BackgroundAnimation) && File.Exists(settings.Media.BackgroundAnimation)) {
                elements.Add(new BackgroundVideo());
            } else if (!string.IsNullOrEmpty(settings.Media.BackgroundImage) && File.Exists(settings.Media.BackgroundImage)) {
                elements.Add(new BackgroundImage());
            }

            // ** Stage ** //

            var stageElements = new List<Element>();
            stageElements.Add(new TapPoints());

            var stage = new Stage(stageElements.ToArray());
            elements.Add(stage);

            // Overlays

            elements.Add(new HelpOverlay {
                Text = settings.LocalStrings.PressSpaceToStart,
                Visible = false
            });

            if (settings.SystemUI.FpsOverlay.Use) {
                elements.Add(new FpsOverlay {
                    FillColor = settings.SystemUI.FpsOverlay.TextFill,
                    FontSize = settings.SystemUI.FpsOverlay.FontSize
                });
            }

            if (settings.SystemUI.DebugOverlay.Use) {
                elements.Add(new DebugOverlay {
                    FillColor = settings.SystemUI.DebugOverlay.TextFill,
                    FontSize = settings.SystemUI.DebugOverlay.FontSize
                });
            }

            return elements.ToArray();
        }

    }
}
