using System.Globalization;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Globalization {
    public abstract class CultureSpecificInfo {

        protected CultureSpecificInfo([NotNull] CultureInfo culture) {
            Culture = culture;
        }

        public CultureInfo Culture { get; }

        public abstract TranslationManager TranslationManager { get; }

    }
}
