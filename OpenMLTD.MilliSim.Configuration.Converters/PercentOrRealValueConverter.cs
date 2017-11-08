using System;
using OpenMLTD.MilliSim.Configuration.Primitives;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Configuration.Converters {
    public sealed class PercentOrRealValueConverter : IYamlTypeConverter {

        public bool Accepts(Type type) {
            return type == typeof(PercentOrRealValue);
        }

        public object ReadYaml(IParser parser, Type type) {
            if (parser.Current == null) {
                return null;
            }

            var scalar = (Scalar)parser.Current;
            var str = scalar.Value;
            if (string.IsNullOrWhiteSpace(str)) {
                return default(PercentOrRealValue);
            }

            str = str.Trim();
            PercentOrRealValue val;
            if (str.EndsWith("%")) {
                var f = Convert.ToSingle(str.Substring(0, str.Length - 1));
                val = new PercentOrRealValue {
                    IsPercentage = true,
                    RawValue = f
                };
            } else {
                var f = Convert.ToSingle(str);
                val = new PercentOrRealValue {
                    IsPercentage = false,
                    RawValue = f
                };
            }
            parser.MoveNext();
            return val;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type) {
            throw new NotImplementedException();
        }

    }
}
