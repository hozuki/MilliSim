using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Configuration.Entities;

namespace OpenMLTD.MilliSim.Configuration {
    public abstract class ConfigurationStore : IReadOnlyDictionary<Type, ConfigBase> {

        public IEnumerator<KeyValuePair<Type, ConfigBase>> GetEnumerator() {
            return _configurations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public int Count => _configurations.Count;

        public bool ContainsKey(Type key) {
            return _configurations.ContainsKey(key);
        }

        public bool TryGetValue(Type key, out ConfigBase value) {
            return _configurations.TryGetValue(key, out value);
        }

        public bool TryGetValue<T>(out T value) where T : ConfigBase {
            var b = TryGetValue(typeof(T), out var val);
            value = (T)val;

            return b;
        }

        public ConfigBase this[Type key] => _configurations[key];

        public T Get<T>() where T : ConfigBase {
            var t = typeof(T);
            return (T)_configurations[t];
        }

        public ConfigBase Get([NotNull] Type type) {
            if (!type.IsSubclassOf(typeof(ConfigBase))) {
                throw new ArgumentException($"Trying to get configuration whose type is '{type.FullName}' from configuration store. The type does not inherit from ConfigBase.");
            }
            return _configurations[type];
        }

        public IEnumerable<Type> Keys => _configurations.Keys;

        public IEnumerable<ConfigBase> Values => _configurations.Values;

        protected Dictionary<Type, ConfigBase> Configurations => _configurations;

        private readonly Dictionary<Type, ConfigBase> _configurations = new Dictionary<Type, ConfigBase>();

    }
}

