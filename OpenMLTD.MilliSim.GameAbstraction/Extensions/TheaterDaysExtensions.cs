using JetBrains.Annotations;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;

namespace OpenMLTD.MilliSim.GameAbstraction.Extensions {
    public static class TheaterDaysExtensions {

        [NotNull]
        public static TheaterDaysBase AsTheaterDays([NotNull] this GameBase game) {
            return (TheaterDaysBase)game;
        }

        [CanBeNull]
        public static T FindSingleElement<T>([NotNull] this TheaterDaysBase days) where T : class, IComponent {
            return days.Stage.FindOrNull<T>();
        }

    }
}
