using System.Globalization;

namespace OpenMLTD.MilliSim.Configuration.Primitives {
    public struct PercentOrRealValue {

        public PercentOrRealValue(float rawValue, bool isPercentage) {
            RawValue = rawValue;
            IsPercentage = isPercentage;
        }

        /// <summary>
        /// Gets/sets the raw value. This value can be interpreted as a real value, or a percentage, regarding <see cref="IsPercentage"/>.
        /// </summary>
        public float RawValue { get; set; }

        /// <summary>
        /// Gets/sets whether the value stored in <see cref="RawValue"/> should be regarded as a percentage.
        /// </summary>
        public bool IsPercentage { get; set; }

        /// <summary>
        /// Gets the real value.
        /// </summary>
        public float Value {
            get {
                if (IsPercentage) {
                    return RawValue / 100;
                } else {
                    return RawValue;
                }
            }
        }

        public static explicit operator float(PercentOrRealValue value) {
            return value.Value;
        }

        /// <summary>
        /// Gets string representation of this <see cref="PercentOrRealValue"/>.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString() {
            var s = RawValue.ToString(CultureInfo.InvariantCulture);
            if (IsPercentage) {
                s += "%";
            }
            return s;
        }

        /// <summary>
        /// Calculates the actual value according to a reference.
        /// </summary>
        /// <param name="reference">The reference value. It will be used if this <see cref="PercentOrRealValue"/> is a percentage.</param>
        /// <returns>Actual value.</returns>
        public float ToActualValue(float reference) {
            if (IsPercentage) {
                return reference * Value;
            } else {
                return Value;
            }
        }

    }
}
