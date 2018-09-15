using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core {
    /// <summary>
    /// An interface indicating that an object can be cloned.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    public interface ICloneable<out T> {

        /// <summary>
        /// Returns a exact copy of this <see cref="ICloneable{T}"/>.
        /// </summary>
        /// <returns>The exact copy.</returns>
        [NotNull]
        T Clone();

    }
}
