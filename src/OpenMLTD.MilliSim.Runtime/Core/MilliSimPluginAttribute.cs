using System;
using System.Composition;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core {
    /// <summary>
    /// An attribute indicating a class or structure is exported as a MilliSim plugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class MilliSimPluginAttribute : ExportAttribute {

        /// <summary>
        /// Creates a new <see cref="MilliSimPluginAttribute"/> instance.
        /// </summary>
        private MilliSimPluginAttribute()
            : base(typeof(IMilliSimPlugin)) {
        }

        // ReSharper disable once UnusedParameter.Local
        /// <inheritdoc />
        /// <summary>
        /// Creates a new <see cref="MilliSimPluginAttribute" /> instance.
        /// </summary>
        /// <param name="literalType">The literal type. Just for reminding the actual exported type. Only used as an indicator for the time being.</param>
        public MilliSimPluginAttribute([NotNull] Type literalType)
            : this() {
        }

    }
}
