using JetBrains.Annotations;
using OpenMLTD.MilliSim.Rendering.Extensions;
using OpenMLTD.MilliSim.Theater.Elements;

namespace OpenMLTD.MilliSim.Theater.Extensions {
    public static class TheaterDaysExtensions {

        [CanBeNull]
        public static BackgroundVideo GetBackgroundVideo(this TheaterDays days) {
            return days.Elements.FindOrNull<BackgroundVideo>();
        }

        [CanBeNull]
        public static BackgroundImage GetBackgroundImage(this TheaterDays days) {
            return days.Elements.FindOrNull<BackgroundImage>();
        }

        [CanBeNull]
        public static HelpOverlay GetHelpOverlay(this TheaterDays days) {
            return days.Elements.FindOrNull<HelpOverlay>();
        }

        [CanBeNull]
        public static DebugOverlay GetDebugOverlay(this TheaterDays days) {
            return days.Elements.FindOrNull<DebugOverlay>();
        }

        [CanBeNull]
        public static FpsOverlay GetFpsOverlay(this TheaterDays days) {
            return days.Elements.FindOrNull<FpsOverlay>();
        }

    }
}
