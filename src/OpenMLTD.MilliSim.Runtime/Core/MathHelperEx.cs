using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Core.Extensions;

namespace OpenMLTD.MilliSim.Core {
    /// <summary>
    /// Provides more functionalities in addition to <see cref="Math"/> and <see cref="MathHelper"/>.
    /// </summary>
    public static class MathHelperEx {

        /// <summary>
        /// Perform linear interpolation between two <see cref="float"/>s.
        /// </summary>
        /// <param name="fromValue">Value to interpolate from.</param>
        /// <param name="toValue">Value to interpolate to.</param>
        /// <param name="t">A value between 0 and 1, indicating how much should move from <see cref="fromValue"/> to <see cref="toValue"/>.</param>
        /// <returns>Interpolated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float fromValue, float toValue, float t) {
            return fromValue * (1 - t) + toValue * t;
        }

        /// <summary>
        /// Perform linear interpolation between two <see cref="double"/>s.
        /// </summary>
        /// <param name="fromValue">Value to interpolate from.</param>
        /// <param name="toValue">Value to interpolate to.</param>
        /// <param name="t">A value between 0 and 1, indicating how much should move from <see cref="fromValue"/> to <see cref="toValue"/>.</param>
        /// <returns>Interpolated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Lerp(double fromValue, double toValue, double t) {
            return fromValue * (1 - t) + toValue * t;
        }

        /// <summary>
        /// Perform linear interpolation between two <see cref="float"/>s.
        /// </summary>
        /// <param name="fromValue">Value to interpolate from.</param>
        /// <param name="toValue">Value to interpolate to.</param>
        /// <param name="t">A value between 0 and 1, indicating how much should move from <see cref="fromValue"/> to <see cref="toValue"/>.</param>
        /// <returns>Interpolated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpTo(this float fromValue, float toValue, float t) {
            return Lerp(fromValue, toValue, t);
        }

        /// <summary>
        /// Perform linear interpolation between two <see cref="double"/>s.
        /// </summary>
        /// <param name="fromValue">Value to interpolate from.</param>
        /// <param name="toValue">Value to interpolate to.</param>
        /// <param name="t">A value between 0 and 1, indicating how much should move from <see cref="fromValue"/> to <see cref="toValue"/>.</param>
        /// <returns>Interpolated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double LerpTo(this double fromValue, double toValue, double t) {
            return Lerp(fromValue, toValue, t);
        }

        /// <summary>
        /// A shorthand to of <see cref="Math.Round(double)"/> and rounds a <see cref="float"/> to an <see cref="int"/>.
        /// </summary>
        /// <param name="value">The value to round.</param>
        /// <returns>The rounded <see cref="int"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Round(float value) {
            return (int)Math.Round(value);
        }

        /// <summary>
        /// Calculates the coordinate of a point on a cubic Bezier curve.
        /// </summary>
        /// <param name="x1">X coordinate of the start point.</param>
        /// <param name="y1">Y coordinate of the start point.</param>
        /// <param name="cx1">X coordinate of the first control point.</param>
        /// <param name="cy1">Y coordinate of the first control point.</param>
        /// <param name="cx2">X coordinate of the second control point.</param>
        /// <param name="cy2">Y coordinate of the second control point.</param>
        /// <param name="x2">X coordinate of the end point.</param>
        /// <param name="y2">Y coordinate of the end point.</param>
        /// <param name="t">A value between 0 and 1 representing how much it travels from start point to end point.</param>
        /// <returns>The coordinate of the point.</returns>
        public static (double X, double Y) CubicBezier(double x1, double y1, double cx1, double cy1, double cx2, double cy2, double x2, double y2, double t) {
            t = Clamp(t, 0, 1);
            var tm = 1 - t;
            var tm2 = tm * tm;
            var tm3 = tm * tm2;
            var t2 = t * t;
            var t3 = t2 * t;
            var x = tm3 * x1 + 3 * tm2 * t * cx1 + 3 * tm * t2 * cx2 + t3 * x2;
            var y = tm3 * y1 + 3 * tm2 * t * cy1 + 3 * tm * t2 * cy2 + t3 * y2;
            return (x, y);
        }

        /// <summary>
        /// Calculates the coordinate of a point on a cubic Bezier curve.
        /// </summary>
        /// <param name="x1">X coordinate of the start point.</param>
        /// <param name="y1">Y coordinate of the start point.</param>
        /// <param name="cx1">X coordinate of the first control point.</param>
        /// <param name="cy1">Y coordinate of the first control point.</param>
        /// <param name="cx2">X coordinate of the second control point.</param>
        /// <param name="cy2">Y coordinate of the second control point.</param>
        /// <param name="x2">X coordinate of the end point.</param>
        /// <param name="y2">Y coordinate of the end point.</param>
        /// <param name="t">A value between 0 and 1 representing how much it travels from start point to end point.</param>
        /// <returns>The coordinate of the point.</returns>
        public static (float X, float Y) CubicBezier(float x1, float y1, float cx1, float cy1, float cx2, float cy2, float x2, float y2, float t) {
            t = MathHelper.Clamp(t, 0, 1);
            var tm = 1 - t;
            var tm2 = tm * tm;
            var tm3 = tm * tm2;
            var t2 = t * t;
            var t3 = t2 * t;
            var x = tm3 * x1 + 3 * tm2 * t * cx1 + 3 * tm * t2 * cx2 + t3 * x2;
            var y = tm3 * y1 + 3 * tm2 * t * cy1 + 3 * tm * t2 * cy2 + t3 * y2;
            return (x, y);
        }

        /// <summary>
        /// Clamps a value between <see cref="min"/> and <see cref="max"/>.
        /// </summary>
        /// <param name="v">The value to clamp.</param>
        /// <param name="min">Lower bound (reachable).</param>
        /// <param name="max">Upper bound (reachable).</param>
        /// <returns>Clamped value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(double v, double min, double max) {
            return v < min ? min : (v > max ? max : v);
        }

        /// <summary>
        /// Provides a global <see cref="System.Random"/> instance.
        /// </summary>
        [NotNull]
        public static readonly Random Random = new Random();

    }
}
