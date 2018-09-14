using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Globalization {
    public abstract class TranslationManager {

        protected TranslationManager([NotNull] CultureSpecificInfo cultureSpecificInfo) {
            CultureSpecificInfo = cultureSpecificInfo;
        }

        [NotNull]
        public string Get([NotNull] string key) {
            return _translations[key];
        }

        public bool ContainsKey([NotNull] string key) {
            return _translations.ContainsKey(key);
        }

        public CultureSpecificInfo CultureSpecificInfo { get; }

        protected Dictionary<string, string> Translations => _translations;

        private readonly Dictionary<string, string> _translations = new Dictionary<string, string>();

    }
}
