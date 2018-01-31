using System;
using System.Drawing;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Configuration.Converters {
    public sealed class SizeFConverter : IYamlTypeConverter {

        public bool Accepts(Type type) {
            return type == typeof(SizeF);
        }

        public object ReadYaml(IParser parser, Type type) {
            if (parser.Current == null) {
                return null;
            }

            var scalar = (Scalar)parser.Current;
            var str = scalar.Value;
            if (string.IsNullOrWhiteSpace(str)) {
                return default(SizeF);
            }

            str = str.Trim().ToLowerInvariant();
            if (str.Count(ch => ch == 'x') != 1) {
                throw new FormatException("Invalid SizeF format.");
            }

            var sizeStrings = str.Split('x');
            var val1 = Convert.ToSingle(sizeStrings[0]);
            var val2 = Convert.ToSingle(sizeStrings[1]);

            var size = new SizeF(val1, val2);

            parser.MoveNext();

            return size;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type) {
            throw new NotImplementedException();
        }

    }
}
