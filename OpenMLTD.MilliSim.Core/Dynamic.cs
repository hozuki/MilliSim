using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenMLTD.MilliSim.Core {
    public class Dynamic : IDynamic {

        protected Dynamic() {
        }

        public virtual object GetValue(string key) {
            return _options[key];
        }

        public virtual void SetValue(string key, object value) {
            _options[key] = value;
        }

        public virtual T GetValue<T>(string key) {
            return (T)GetValue(key);
        }

        public virtual void SetValue<T>(string key, T value) {
            _options[key] = value;
        }

        public static readonly Dynamic Empty = new EmptyDynamic();

        #region IDictionary<TKey, TValue>
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => _options.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _options.GetEnumerator();

        int IReadOnlyCollection<KeyValuePair<string, object>>.Count => _options.Count;

        public bool ContainsKey(string key) {
            return _options.ContainsKey(key);
        }

        bool IReadOnlyDictionary<string, object>.TryGetValue(string key, out object value) {
            return _options.TryGetValue(key, out value);
        }

        object IReadOnlyDictionary<string, object>.this[string key] => _options[key];

        IEnumerable<string> IReadOnlyDictionary<string, object>.Keys => _options.Keys;

        IEnumerable<object> IReadOnlyDictionary<string, object>.Values => _options.Values;
        #endregion

        private readonly Dictionary<string, object> _options = new Dictionary<string, object>();

        private sealed class EmptyDynamic : Dynamic {

            public override void SetValue(string key, object value) {
                throw new NotSupportedException();
            }

            public override void SetValue<T>(string key, T value) {
                throw new NotSupportedException();
            }

        }

    }
}
