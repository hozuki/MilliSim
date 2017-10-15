using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core.Extensions;

namespace OpenMLTD.MilliSim.Core {
    /// <inheritdoc cref="IDynamic" />
    /// <inheritdoc cref="ICloneable{T}"/>
    /// <summary>
    /// A basic implementation of <see cref="T:OpenMLTD.MilliSim.Core.IDynamic" />.
    /// </summary>
    public class Dynamic : IDynamic, ICloneable<Dynamic> {

        public object GetValue(string key) {
            return _options[key];
        }

        public void SetValue(string key, object value) {
            _options[key] = value;
        }

        public virtual T GetValue<T>(string key) {
            return (T)GetValue(key);
        }

        public virtual void SetValue<T>(string key, T value) {
            _options[key] = value;
        }

        public bool RemoveValue(string key) {
            return _options.Remove(key);
        }

        public T Clone<T>() where T : Dynamic, new() {
            return (T)Clone(typeof(T));
        }

        public Dynamic Clone() {
            return Clone(typeof(Dynamic));
        }

        public Dynamic Clone([NotNull] Type dynamicType) {
            if (dynamicType == null) {
                throw new ArgumentNullException(nameof(dynamicType));
            }

            var td = typeof(Dynamic);
            if (dynamicType != td && !dynamicType.IsSubclassOf(td)) {
                throw new ArgumentException("Specified type is not a Dynamic or a subclass of Dynamic.");
            }

            var ctor = dynamicType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
            var dyn = (Dynamic)ctor.Invoke(ReflectionHelper.EmptyObjects);

            var gicType = typeof(ICloneable<>);
            var cloneMethod = gicType.GetMethod(nameof(ICloneable<object>.Clone), BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);

            foreach (var (k, v) in this) {
                if (v is ICloneable cl1) {
                    dyn.SetValue(k, cl1.Clone());
                } else {
                    var ty = v.GetType();
                    if (ty.ImplementsGenericInterface(gicType)) {
                        var cv = cloneMethod.Invoke(v, ReflectionHelper.EmptyObjects);
                        dyn.SetValue(k, cv);
                    } else {
                        dyn.SetValue(k, v);
                    }
                }
            }

            return dyn;
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

        IDynamic ICloneable<IDynamic>.Clone() {
            return Clone();
        }

        private readonly Dictionary<string, object> _options = new Dictionary<string, object>();

        private sealed class EmptyDynamic : Dynamic {

            public override void SetValue<T>(string key, T value) {
                throw new NotSupportedException();
            }

        }

    }
}
