using System;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core {
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class AssemblyBuildTimeAttribute : Attribute {

        public AssemblyBuildTimeAttribute([CanBeNull] string buildTimeString) {
            BuildTimeString = buildTimeString ?? string.Empty;
        }

        [NotNull]
        public string BuildTimeString { get; }

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
