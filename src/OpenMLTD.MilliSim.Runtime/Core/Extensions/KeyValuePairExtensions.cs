using System;
using System.Collections.Generic;

namespace OpenMLTD.MilliSim.Core.Extensions {
    /// <summary>
    /// <see cref="KeyValuePair{TKey,TValue}"/> extension methods.
    /// </summary>
    public static class KeyValuePairExtensions {

        /// <summary>
        /// Deconstructs a <see cref="KeyValuePair{TKey,TValue}"/> to a <see cref="ValueTuple{TKey,TValue}"/>.
        /// With this method, <see cref="KeyValuePair{TKey,TValue}"/> can be used in tuple deconstruction semantics.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="kv">The <see cref="KeyValuePair{TKey,TValue}"/> to deconstruct.</param>
        /// <param name="v0">First member of the <see cref="ValueTuple{TKey,TValue}"/>.</param>
        /// <param name="v1">Second member of the <see cref="ValueTuple{TKey,TValue}"/>.</param>
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kv, out TKey v0, out TValue v1) {
            v0 = kv.Key;
            v1 = kv.Value;
        }

        /// <summary>
        /// Deconstructs a <see cref="KeyValuePair{TKey,TValue}"/> to a <see cref="ValueTuple{TKey,T1,T2}"/>.
        /// With this method, <see cref="KeyValuePair{TKey,TValue}"/> can be used in tuple deconstruction semantics.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="T1">First type of the value tuple.</typeparam>
        /// <typeparam name="T2">Second type of the value tuple.</typeparam>
        /// <param name="kv">The <see cref="KeyValuePair{TKey,TValue}"/> to deconstruct.</param>
        /// <param name="v0">First member of the <see cref="ValueTuple{TKey,T1,T2}"/>.</param>
        /// <param name="v1">Second member of the <see cref="ValueTuple{TKey,T1,T2}"/>.</param>
        /// <param name="v2">Third member of the <see cref="ValueTuple{TKey,T1,T2}"/>.</param>
        public static void Deconstruct<TKey, T1, T2>(this KeyValuePair<TKey, (T1, T2)> kv, out TKey v0, out T1 v1, out T2 v2) {
            v0 = kv.Key;
            v1 = kv.Value.Item1;
            v2 = kv.Value.Item2;
        }

        /// <summary>
        /// Deconstructs a <see cref="KeyValuePair{TKey,TValue}"/> to a <see cref="ValueTuple{TKey,T1,T2,T3}"/>.
        /// With this method, <see cref="KeyValuePair{TKey,TValue}"/> can be used in tuple deconstruction semantics.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="T1">First type of the value tuple.</typeparam>
        /// <typeparam name="T2">Second type of the value tuple.</typeparam>
        /// <typeparam name="T3">Third type of the value tuple.</typeparam>
        /// <param name="kv">The <see cref="KeyValuePair{TKey,TValue}"/> to deconstruct.</param>
        /// <param name="v0">First member of the <see cref="ValueTuple{TKey,T1,T2,T3}"/>.</param>
        /// <param name="v1">Second member of the <see cref="ValueTuple{TKey,T1,T2,T3}"/>.</param>
        /// <param name="v2">Third member of the <see cref="ValueTuple{TKey,T1,T2,T3}"/>.</param>
        /// /// <param name="v3">Fourth member of the <see cref="ValueTuple{TKey,T1,T2,T3}"/>.</param>
        public static void Deconstruct<TKey, T1, T2, T3>(this KeyValuePair<TKey, (T1, T2, T3)> kv, out TKey v0, out T1 v1, out T2 v2, out T3 v3) {
            v0 = kv.Key;
            v1 = kv.Value.Item1;
            v2 = kv.Value.Item2;
            v3 = kv.Value.Item3;
        }

        /// <summary>
        /// Deconstructs a <see cref="KeyValuePair{TKey,TValue}"/> to a <see cref="ValueTuple{TKey,T1,T2,T3,T4}"/>.
        /// With this method, <see cref="KeyValuePair{TKey,TValue}"/> can be used in tuple deconstruction semantics.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="T1">First type of the value tuple.</typeparam>
        /// <typeparam name="T2">Second type of the value tuple.</typeparam>
        /// <typeparam name="T3">Third type of the value tuple.</typeparam>
        /// <typeparam name="T4">Fourth type of the value tuple.</typeparam>
        /// <param name="kv">The <see cref="KeyValuePair{TKey,TValue}"/> to deconstruct.</param>
        /// <param name="v0">First member of the <see cref="ValueTuple{TKey,T1,T2,T3,T4}"/>.</param>
        /// <param name="v1">Second member of the <see cref="ValueTuple{TKey,T1,T2,T3,T4}"/>.</param>
        /// <param name="v2">Third member of the <see cref="ValueTuple{TKey,T1,T2,T3,T4}"/>.</param>
        /// /// <param name="v3">Fourth member of the <see cref="ValueTuple{TKey,T1,T2,T3,T4}"/>.</param>
        /// <param name="v4">Fifth member of the <see cref="ValueTuple{TKey,T1,T2,T3,T4}"/>.</param>
        public static void Deconstruct<TKey, T1, T2, T3, T4>(this KeyValuePair<TKey, (T1, T2, T3, T4)> kv, out TKey v0, out T1 v1, out T2 v2, out T3 v3, out T4 v4) {
            v0 = kv.Key;
            v1 = kv.Value.Item1;
            v2 = kv.Value.Item2;
            v3 = kv.Value.Item3;
            v4 = kv.Value.Item4;
        }

    }
}
