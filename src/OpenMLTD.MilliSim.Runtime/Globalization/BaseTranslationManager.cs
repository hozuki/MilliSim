using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Globalization {
    /// <summary>
    /// The base class of translation managers. This class must be inherited.
    /// </summary>
    public abstract class BaseTranslationManager {

        /// <summary>
        /// Creates a new <see cref="BaseTranslationManager"/> from a given <see cref="OpenMLTD.MilliSim.Globalization.CultureSpecificInfo"/>.
        /// </summary>
        /// <param name="cultureSpecificInfo">The <see cref="OpenMLTD.MilliSim.Globalization.CultureSpecificInfo"/> to create from.</param>
        protected BaseTranslationManager([NotNull] CultureSpecificInfo cultureSpecificInfo) {
            CultureSpecificInfo = cultureSpecificInfo;
        }

        /// <summary>
        /// Gets a translated text by key.
        /// </summary>
        /// <param name="key">The key to translation entry.</param>
        /// <returns>Translated text.</returns>
        [NotNull]
        public string Get([NotNull] string key) {
            return _translations[key];
        }

        /// <summary>
        /// Checks whether a key exists in the translation table.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns><see langword="true"/> if the key exists, otherwise <see langword="false"/>.</returns>
        public bool ContainsKey([NotNull] string key) {
            return _translations.ContainsKey(key);
        }

        /// <summary>
        /// Gets the <see cref="OpenMLTD.MilliSim.Globalization.CultureSpecificInfo"/> this <see cref="BaseTranslationManager"/> associates to.
        /// </summary>
        public CultureSpecificInfo CultureSpecificInfo { get; }

        /// <summary>
        /// Gets the translation table.
        /// </summary>
        protected Dictionary<string, string> Translations => _translations;

        private readonly Dictionary<string, string> _translations = new Dictionary<string, string>();

    }
}
