using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Configuration.Entities;

namespace OpenMLTD.MilliSim.Configuration {
    /// <inheritdoc />
    /// <summary>
    /// A configuration store is a storage class for multiple configuration items of different types.
    /// </summary>
    public abstract class BaseConfigurationStore : IReadOnlyDictionary<Type, ConfigBase> {

        public IEnumerator<KeyValuePair<Type, ConfigBase>> GetEnumerator() {
            return _configurations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public int Count => _configurations.Count;

        public bool ContainsKey([NotNull] Type key) {
            return _configurations.ContainsKey(key);
        }

        public bool TryGetValue(Type key, [CanBeNull] out ConfigBase value) {
            return _configurations.TryGetValue(key, out value);
        }

        /// <summary>
        /// Try to get a value by specifying its type.
        /// </summary>
        /// <typeparam name="T">The type of the value. It should be a subclass of <see cref="ConfigBase"/>.</typeparam>
        /// <param name="value">Retrieved value, or <see langword="null"/> if nothing is found.</param>
        /// <returns>Retrieved value.</returns>
        public bool TryGetValue<T>([CanBeNull] out T value) where T : ConfigBase {
            var b = TryGetValue(typeof(T), out var val);
            value = (T)val;

            return b;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get a value by specifying its type.
        /// </summary>
        /// <param name="type">The type of the value. It should be a subclass of <see cref="ConfigBase"/>.</param>
        /// <returns>Retrieved value.</returns>
        /// <exception cref="ArgumentException">The specified type is not a subclass of <see cref="ConfigBase"/>.</exception>
        [NotNull]
        public ConfigBase this[[NotNull] Type key] => Get(key);

        /// <summary>
        /// Get a value by specifying its type.
        /// </summary>
        /// <typeparam name="T">The type of the value. It should be a subclass of <see cref="ConfigBase"/>.</typeparam>
        /// <returns>Retrieved value.</returns>
        /// <exception cref="ArgumentException">The specified type is not a subclass of <see cref="ConfigBase"/>.</exception>
        public T Get<T>() where T : ConfigBase {
            var t = typeof(T);
            return (T)Get(t);
        }

        /// <summary>
        /// Get a value by specifying its type.
        /// </summary>
        /// <param name="type">The type of the value. It should be a subclass of <see cref="ConfigBase"/>.</param>
        /// <returns>Retrieved value.</returns>
        /// <exception cref="ArgumentException">The specified type is not a subclass of <see cref="ConfigBase"/>.</exception>
        [NotNull]
        public ConfigBase Get([NotNull] Type type) {
            if (!type.IsSubclassOf(typeof(ConfigBase))) {
                throw new ArgumentException($"Trying to get configuration whose type is '{type.FullName}' from configuration store. The type does not inherit from ConfigBase.");
            }
            return _configurations[type];
        }

        public IEnumerable<Type> Keys => _configurations.Keys;

        public IEnumerable<ConfigBase> Values => _configurations.Values;

        /// <summary>
        /// Gets the configuration table.
        /// </summary>
        protected Dictionary<Type, ConfigBase> Configurations => _configurations;

        private readonly Dictionary<Type, ConfigBase> _configurations = new Dictionary<Type, ConfigBase>();

    }
}

