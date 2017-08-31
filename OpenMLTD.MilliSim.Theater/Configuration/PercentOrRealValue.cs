using System;
using System.Globalization;

namespace OpenMLTD.MilliSim.Theater.Configuration {
    public struct PercentOrRealValue {

        public float Value { get; set; }

        public bool IsPercentage { get; set; }

        public static implicit operator float(PercentOrRealValue value) {
            if (value.IsPercentage) {
                return value.Value / 100;
            } else {
                return value.Value;
            }
        }

        public override string ToString() {
            var s = Value.ToString(CultureInfo.InvariantCulture);
            if (IsPercentage) {
                s += "%";
            }
            return s;
        }

        public static float operator *(float f, PercentOrRealValue value) {
            return value * f;
        }

        public static float operator *(PercentOrRealValue value, float f) {
            if (value.IsPercentage) {
                if (Math.Abs(value.Value) < 1000) {
                    return (value.Value * f) / 100;
                } else {
                    return (value.Value / 100) * f;
                }
            } else {
                return value.Value * f;
            }
        }

    }
}
