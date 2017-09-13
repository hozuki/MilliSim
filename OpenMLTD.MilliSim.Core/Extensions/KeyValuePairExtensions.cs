using System.Collections.Generic;

namespace OpenMLTD.MilliSim.Core.Extensions {
    public static class KeyValuePairExtensions {

        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kv, out TKey v0, out TValue v1) {
            v0 = kv.Key;
            v1 = kv.Value;
        }

        public static void Deconstruct<TKey, T1, T2>(this KeyValuePair<TKey, (T1, T2)> kv, out TKey v0, out T1 v1, out T2 v2) {
            v0 = kv.Key;
            v1 = kv.Value.Item1;
            v2 = kv.Value.Item2;
        }

        public static void Deconstruct<TKey, T1, T2, T3>(this KeyValuePair<TKey, (T1, T2, T3)> kv, out TKey v0, out T1 v1, out T2 v2, out T3 v3) {
            v0 = kv.Key;
            v1 = kv.Value.Item1;
            v2 = kv.Value.Item2;
            v3 = kv.Value.Item3;
        }

        public static void Deconstruct<TKey, T1, T2, T3, T4>(this KeyValuePair<TKey, (T1, T2, T3, T4)> kv, out TKey v0, out T1 v1, out T2 v2, out T3 v3, out T4 v4) {
            v0 = kv.Key;
            v1 = kv.Value.Item1;
            v2 = kv.Value.Item2;
            v3 = kv.Value.Item3;
            v4 = kv.Value.Item4;
        }

    }
}
