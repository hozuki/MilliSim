using System;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core {
    /// <inheritdoc />
    /// <summary>
    /// Applies code name to assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class MilliSimCodeNameAttribute : Attribute {

        /// <summary>
        /// Creates a new <see cref="MilliSimCodeNameAttribute"/> instance.
        /// </summary>
        /// <param name="codeName">The code name.</param>
        public MilliSimCodeNameAttribute([CanBeNull] string codeName) {
            CodeName = string.IsNullOrWhiteSpace(codeName) ? DefaultCodeName : codeName;
        }

        /// <summary>
        /// Gets the code name.
        /// </summary>
        [NotNull]
        public string CodeName { get; }

        /// <summary>
        /// Default code name.
        /// </summary>
        public static readonly string DefaultCodeName = "Unknown";

    }
}
