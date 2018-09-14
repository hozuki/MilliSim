using System.Globalization;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Globalization;

namespace OpenMLTD.TheaterDays.Globalization {
    public sealed class TheaterDaysCultureSpecificInfo : CultureSpecificInfo {

        public TheaterDaysCultureSpecificInfo([NotNull] CultureInfo culture)
            : base(culture) {
            TranslationManager = new TheaterDaysTranslationManager(this);
        }

        public override TranslationManager TranslationManager { get; }

    }
}
