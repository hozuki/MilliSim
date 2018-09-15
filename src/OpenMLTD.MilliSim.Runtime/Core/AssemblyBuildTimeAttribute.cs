using System;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core {
    /// <inheritdoc />
    /// <summary>
    /// Indicates the build time of the assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class AssemblyBuildTimeAttribute : Attribute {

        public AssemblyBuildTimeAttribute([CanBeNull] string buildTimeString) {
            BuildTimeString = buildTimeString ?? string.Empty;
        }

        /// <summary>
        /// Gets the build time string.
        /// </summary>
        [NotNull]
        public string BuildTimeString { get; }

        /// <summary>
        /// Gets the build time of the assembly.
        /// </summary>
        public DateTime BuildTime {
            get {
                if (_buildTime != null) {
                    return _buildTime.Value;
                }

                if (string.IsNullOrEmpty(BuildTimeString)) {
                    _buildTime = DateTime.MinValue;
                } else {
                    _buildTime = DateTime.Parse(BuildTimeString);
                }

                return _buildTime.Value;
            }
        }

        private DateTime? _buildTime;

    }
}
