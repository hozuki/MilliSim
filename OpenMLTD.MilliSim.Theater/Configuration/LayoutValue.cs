using System;
using System.Globalization;

namespace OpenMLTD.MilliSim.Theater.Configuration {
    public struct LayoutValue {

        public float Value { get; set; }

        public bool IsPercentage { get; set; }

        public override string ToString() {
            var s = Value.ToString(CultureInfo.InvariantCulture);
            if (IsPercentage) {
                s += "%";
            }
            return s;
        }

        public static float operator *(float f, LayoutValue layoutValue) {
            return layoutValue * f;
        }

        public static float operator *(LayoutValue layoutValue, float f) {
            if (layoutValue.IsPercentage) {
                if (Math.Abs(layoutValue.Value) < 1000) {
                    return (layoutValue.Value * f) / 100;
                } else {
                    return (layoutValue.Value / 100) * f;
                }
            } else {
                return layoutValue.Value * f;
            }
        }

    }
}
