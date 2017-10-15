using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core {
    /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}" />
    /// <inheritdoc cref="ICloneable{T}"/>
    /// <summary>
    /// Represents a dynamic object.
    /// </summary>
    public interface IDynamic : IReadOnlyDictionary<string, object>, ICloneable<IDynamic> {

        /// <summary>
        /// Gets the value by specified key.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <returns>Retrieved value.</returns>
        [CanBeNull]
        object GetValue([NotNull] string key);

        /// <summary>
        /// Sets the value by specified key.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The value to set.</param>
        void SetValue([NotNull] string key, [CanBeNull] object value);

        /// <summary>
        /// Gets the value by specified key.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="key">The key for this value.</param>
        /// <remarks>The type <see cref="T"/> and the value's real type must match. Otherwise a <see cref="InvalidCastException"/> will be thrown.</remarks>
        /// <returns>Retrieved value</returns>
        [CanBeNull]
        T GetValue<T>([NotNull] string key);

        /// <summary>
        /// Sets the value by specified key.
        /// </summary>
        /// <typeparam name="T">The type of the value to set.</typeparam>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The value to set.</param>
        void SetValue<T>([NotNull] string key, [CanBeNull] T value);

        /// <summary>
        /// Removes a value from the <see cref="IDynamic"/>.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <returns>Returns <see langword="true"/> if a value with specified key is found. Otherwise, <see langword="false"/>.</returns>
        bool RemoveValue([NotNull] string key);

    }
}
