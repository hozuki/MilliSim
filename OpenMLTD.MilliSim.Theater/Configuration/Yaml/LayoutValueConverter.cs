using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Theater.Configuration.Yaml {
    public sealed class LayoutValueConverter : IYamlTypeConverter {

        public bool Accepts(Type type) {
            return type == typeof(LayoutValue);
        }

        public object ReadYaml(IParser parser, Type type) {
            if (parser.Current == null) {
                return null;
            }
            var scalar = (Scalar)parser.Current;
            var str = scalar.Value.Trim();
            LayoutValue val;
            if (str.EndsWith("%")) {
                var f = Convert.ToSingle(str.Substring(0, str.Length - 1));
                val = new LayoutValue {
                    IsPercentage = true,
                    Value = f
                };
            } else {
                var f = Convert.ToSingle(str);
                val = new LayoutValue {
                    IsPercentage = false,
                    Value = f
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
