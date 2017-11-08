using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Configuration {
    internal sealed class ConfigurationStoreOld {

        private ConfigurationStoreOld([NotNull] IReadOnlyDictionary<object, object> entries) {
            _entries = entries;
        }

        public static ConfigurationStoreOld FromString([NotNull] string configuration, [NotNull] Deserializer deserializer) {
            var obj = deserializer.Deserialize(configuration, typeof(Dictionary<object, object>));
            if (!(obj is IReadOnlyDictionary<object, object> dict)) {
                throw new SerializationException();
            }
            return new ConfigurationStoreOld(dict);
        }

        public static ConfigurationStoreOld FromFile([NotNull] string path, [NotNull] Deserializer deserializer) {
            return FromFile(path, Encoding.UTF8, deserializer);
        }

        public static ConfigurationStoreOld FromFile([NotNull] string path, [NotNull] Encoding encoding, [NotNull] Deserializer deserializer) {
            using (var fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var reader = new StreamReader(fileStream, encoding)) {
                    return FromFile(reader, deserializer);
                }
            }
        }

        public static ConfigurationStoreOld FromFile([NotNull] TextReader reader, [NotNull] Deserializer deserializer) {
            var obj = deserializer.Deserialize(reader);
            if (!(obj is IReadOnlyDictionary<object, object> dict)) {
                throw new SerializationException();
            }
            return new ConfigurationStoreOld(dict);
        }

        public IReadOnlyList<object> GetList([NotNull] string key) {
            var value = Get(key);
            return (IReadOnlyList<object>)value;
        }

        public IReadOnlyDictionary<object, object> GetDictionary([NotNull] string key) {
            var value = Get(key);
            return (IReadOnlyDictionary<object, object>)value;
        }

        public byte GetByte([NotNull] string key) {
            var value = Get(key);
            return Convert.ToByte(value);
        }

        public sbyte GetSByte([NotNull] string key) {
            var value = Get(key);
            return Convert.ToSByte(value);
        }

        public short GetInt16([NotNull] string key) {
            var value = Get(key);
            return Convert.ToInt16(value);
        }

        public ushort GetUInt16([NotNull] string key) {
            var value = Get(key);
            return Convert.ToUInt16(value);
        }

        public int GetInt32([NotNull] string key) {
            var value = Get(key);
            return Convert.ToInt32(value);
        }

        public uint GetUInt32([NotNull] string key) {
            var value = Get(key);
            return Convert.ToUInt32(value);
        }

        public long GetInt64([NotNull] string key) {
            var value = Get(key);
            return Convert.ToInt64(value);
        }

        public ulong GetUInt64([NotNull] string key) {
            var value = Get(key);
            return Convert.ToUInt64(value);
        }

        public float GetSingle([NotNull] string key) {
            var value = Get(key);
            return Convert.ToSingle(value);
        }

        public double GetDouble([NotNull] string key) {
            var value = Get(key);
            return Convert.ToDouble(value);
        }

        public string GetString([NotNull] string key) {
            var value = Get(key);
            if (value is string s) {
                return s;
            } else {
                throw new InvalidCastException();
            }
        }

        public DateTime GetDateTime([NotNull] string key) {
            var value = Get(key);
            if (value is string s) {
                return DateTime.Parse(s);
            } else {
                throw new InvalidCastException();
            }
        }

        public bool GetBoolean([NotNull] string key) {
            var value = Get(key);
            if (value is string s) {
                return StringToBoolean(s);
            } else if (value is IDictionary<object, object> dict) {
                return dict.Count > 0;
            } else if (value is IList<object> list) {
                return list.Count > 0;
            } else {
                return value != null;
            }
        }

        public object Get([NotNull] string key) {
            var b = TryGet(key, out var value);
            if (!b) {
                throw new KeyNotFoundException();
            }
            return value;
        }

        public T Get<T>([NotNull] string key) {
            var t = typeof(T);
            if (t == typeof(string)) {
                return (T)(object)GetString(key);
            } else if (t == typeof(byte)) {
                return (T)(object)GetByte(key);
            } else if (t == typeof(sbyte)) {
                return (T)(object)GetSByte(key);
            } else if (t == typeof(short)) {
                return (T)(object)GetInt16(key);
            } else if (t == typeof(ushort)) {
                return (T)(object)GetUInt16(key);
            } else if (t == typeof(int)) {
                return (T)(object)GetInt32(key);
            } else if (t == typeof(uint)) {
                return (T)(object)GetUInt32(key);
            } else if (t == typeof(long)) {
                return (T)(object)GetInt64(key);
            } else if (t == typeof(ulong)) {
                return (T)(object)GetUInt64(key);
            } else if (t == typeof(float)) {
                return (T)(object)GetSingle(key);
            } else if (t == typeof(double)) {
                return (T)(object)GetDouble(key);
            } else if (t == typeof(bool)) {
                return (T)(object)GetBoolean(key);
            } else if (t == typeof(DateTime)) {
                return (T)(object)GetDateTime(key);
            } else if (t == typeof(IReadOnlyList<object>)) {
                return (T)GetList(key);
            } else if (t == typeof(IReadOnlyDictionary<object, object>)) {
                return (T)GetDictionary(key);
            } else {
                return (T)Get(key);
            }
        }

        public bool TryGet<T>([NotNull] string key, [CanBeNull] out T value) where T : class {
            var b = TryGet(key, out var v);
            if (!b) {
                value = default;
                return false;
            }

            if (v == null) {
                value = default;
                return true;
            }

            if (v is string s) {
                var t = typeof(T);
                if (t == typeof(string)) {
                    value = (T)(object)s;
                } else if (t == typeof(byte)) {
                    value = (T)(object)Convert.ToByte(s);
                } else if (t == typeof(sbyte)) {
                    value = (T)(object)Convert.ToSByte(s);
                } else if (t == typeof(short)) {
                    value = (T)(object)Convert.ToInt16(s);
                } else if (t == typeof(ushort)) {
                    value = (T)(object)Convert.ToUInt16(s);
                } else if (t == typeof(int)) {
                    value = (T)(object)Convert.ToInt32(s);
                } else if (t == typeof(uint)) {
                    value = (T)(object)Convert.ToUInt32(s);
                } else if (t == typeof(long)) {
                    value = (T)(object)Convert.ToInt64(s);
                } else if (t == typeof(ulong)) {
                    value = (T)(object)Convert.ToUInt64(s);
                } else if (t == typeof(bool)) {
                    value = (T)(object)StringToBoolean(s);
                } else if (t == typeof(float)) {
                    value = (T)(object)Convert.ToSingle(s);
                } else if (t == typeof(double)) {
                    value = (T)(object)Convert.ToDouble(s);
                } else {
                    value = default;
                    return false;
                }
            } else {
                value = (T)v;
            }

            return true;
        }

        public bool TryGet([NotNull] string key, [CanBeNull] out object value) {
            var keys = key.Split(EntrySplitters, StringSplitOptions.RemoveEmptyEntries);
            object current = _entries;

            foreach (var k in keys) {
                switch (current) {
                    case IReadOnlyDictionary<object, object> d:
                        if (d.ContainsKey(k)) {
                            current = d[k];
                        } else {
                            value = null;
                            return false;
                        }

                        break;
                    case IReadOnlyList<object> l:
                        var b = int.TryParse(k, out var index);
                        if (!b) {
                            value = null;
                            return false;
                        }

                        if (0 <= index && index < l.Count) {
                            current = l[index];
                        } else {
                            value = null;
                            return false;
                        }

                        break;
                    default:
                        value = null;
                        return false;
                }
            }

            value = current;
            return true;
        }

        public bool Has([NotNull] string key) {
            var keys = key.Split(EntrySplitters, StringSplitOptions.RemoveEmptyEntries);
            object current = _entries;

            foreach (var k in keys) {
                switch (current) {
                    case IReadOnlyDictionary<object, object> d:
                        if (d.ContainsKey(k)) {
                            current = d[k];
                        } else {
                            return false;
                        }

                        break;
                    case IReadOnlyList<object> l:
                        var b = int.TryParse(k, out var index);
                        if (!b) {
                            return false;
                        }

                        if (0 <= index && index < l.Count) {
                            current = l[index];
                        } else {
                            return false;
                        }

                        break;
                    default:
                        return false;
                }
            }

            return true;
        }

        private static bool StringToBoolean([NotNull] string s) {
            if (string.IsNullOrWhiteSpace(s)) {
                return false;
            }

            var isNumber = double.TryParse(s, out var dbl);
            if (isNumber) {
                return !dbl.Equals(0);
            }

            var lower = s.ToLowerInvariant();
            if (lower == "y" || lower == "yes" || lower == "true" || lower == "on" || lower == "+") {
                return true;
            } else if (lower == "n" || lower == "no" || lower == "false" || lower == "off" || lower == "-") {
                return false;
            } else {
                throw new InvalidCastException();
            }
        }

        private readonly IReadOnlyDictionary<object, object> _entries;
        private static readonly char[] EntrySplitters = {'.'};

    }
}
