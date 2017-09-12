using System;

namespace OpenMLTD.MilliSim.Core.Entities {
    internal sealed class EmptyFlexibleOptions : FlexibleOptions {

        public override void SetValue(string key, object value) {
            throw new NotSupportedException();
        }

        public override void SetValue<T>(string key, T value) {
            throw new NotSupportedException();
        }

    }
}
