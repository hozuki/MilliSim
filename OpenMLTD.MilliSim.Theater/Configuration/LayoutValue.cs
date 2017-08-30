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

    }
}
