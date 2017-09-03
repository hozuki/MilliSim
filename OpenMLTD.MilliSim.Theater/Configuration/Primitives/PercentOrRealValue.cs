using System;
using System.Globalization;

namespace OpenMLTD.MilliSim.Theater.Configuration.Primitives {
    public struct PercentOrRealValue {

        public float RawValue { get; set; }

        public bool IsPercentage { get; set; }

        public float Value {
            get {
                if (IsPercentage) {
                    return RawValue / 100;
                } else {
                    return RawValue;
                }
            }
        }

        public static implicit operator float(PercentOrRealValue value) {
            return value.Value;
        }

        public override string ToString() {
            var s = RawValue.ToString(CultureInfo.InvariantCulture);
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
                if (Math.Abs(value.RawValue) < 1000) {
                    return (value.RawValue * f) / 100;
                } else {
                    return (value.RawValue / 100) * f;
                }
            } else {
                return value.RawValue * f;
            }
        }

    }
}
