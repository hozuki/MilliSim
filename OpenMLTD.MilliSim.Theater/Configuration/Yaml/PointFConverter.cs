using System;
using System.Drawing;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Theater.Configuration.Yaml {
    public sealed class PointFConverter : IYamlTypeConverter {

        public bool Accepts(Type type) {
            return type == typeof(PointF);
        }

        public object ReadYaml(IParser parser, Type type) {
            if (parser.Current == null) {
                return null;
            }

            var scalar = (Scalar)parser.Current;
            var str = scalar.Value;
            if (string.IsNullOrWhiteSpace(str)) {
                return default(PointF);
            }

            str = str.Trim().ToLowerInvariant();
            if (str.Count(ch => ch == 'x') != 1) {
                throw new FormatException("Invalid PointF format.");
            }

            var sizeStrings = str.Split('x');
            var val1 = Convert.ToSingle(sizeStrings[0]);
            var val2 = Convert.ToSingle(sizeStrings[1]);

            var size = new PointF(val1, val2);

            parser.MoveNext();

            return size;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type) {
            throw new NotImplementedException();
        }

    }
}
