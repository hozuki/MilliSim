using JetBrains.Annotations;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;

namespace OpenMLTD.MilliSim.Theater.Extensions {
    public static class TheaterDaysExtensions {

        public static TheaterDays AsTheaterDays(this GameBase game) {
            return (TheaterDays)game;
        }

        [CanBeNull]
        public static T GetSingleElement<T>(this TheaterDays days) where T : class, IElement {
            return days.Stage.FindOrNull<T>();
        }

    }
}
