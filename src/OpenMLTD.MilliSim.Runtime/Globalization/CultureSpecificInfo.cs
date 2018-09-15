using System.Globalization;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Globalization {
    /// <summary>
    /// Represents culture-specific information. This class must be inherited.
    /// </summary>
    public abstract class CultureSpecificInfo {

        /// <summary>
        /// Creates a new <see cref="CultureSpecificInfo"/>.
        /// </summary>
        /// <param name="culture">The culture to create from.</param>
        protected CultureSpecificInfo([NotNull] CultureInfo culture) {
            Culture = culture;
        }

        /// <summary>
        /// Gets the culture that created this <see cref="CultureSpecificInfo"/>.
        /// </summary>
        public CultureInfo Culture { get; }

        /// <summary>
        /// Gets the <see cref="BaseTranslationManager"/> for translation.
        /// </summary>
        public abstract BaseTranslationManager TranslationManager { get; }

    }
}
