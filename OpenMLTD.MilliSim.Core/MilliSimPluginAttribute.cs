using System;
using System.Composition;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class MilliSimPluginAttribute : ExportAttribute {

        /// <summary>
        /// Creates a new <see cref="MilliSimPluginAttribute"/> instance.
        /// </summary>
        public MilliSimPluginAttribute()
            : base(typeof(IMilliSimPlugin)) {
        }

        // ReSharper disable once UnusedParameter.Local
        /// <summary>
        /// Creates a new <see cref="MilliSimPluginAttribute"/> instance.
        /// </summary>
        /// <param name="literalType">The literal type. Just for reminding the actual exported type.</param>
        public MilliSimPluginAttribute([NotNull] Type literalType)
            : this() {
        }

    }
}
