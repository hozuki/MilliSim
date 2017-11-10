using System.Globalization;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Globalization {
    public sealed class CultureSpecificInfo {

        public CultureSpecificInfo([NotNull] CultureInfo culture) {
            Culture = culture;
            TranslationManager = new TranslationManager(this);
        }

        public CultureInfo Culture { get; }

        public TranslationManager TranslationManager { get; }

    }
}
