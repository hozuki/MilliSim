using System;
using System.Linq;
using Microsoft.Xna.Framework;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace OpenMLTD.TheaterDays.Configuration.Converters {
    internal sealed class Vector2Converter : IYamlTypeConverter {

        public bool Accepts(Type type) {
            return type == typeof(Vector2);
        }

        public object ReadYaml(IParser parser, Type type) {
            if (parser.Current == null) {
                return null;
            }

            var scalar = (Scalar)parser.Current;
            var str = scalar.Value;
            if (string.IsNullOrWhiteSpace(str)) {
                return default(Vector2);
            }

            str = str.Trim().ToLowerInvariant();
            if (str.Count(ch => ch == 'x') != 1) {
                throw new FormatException("Invalid Vector2 format.");
            }

            var sizeStrings = str.Split('x');
            var val1 = Convert.ToSingle(sizeStrings[0]);
            var val2 = Convert.ToSingle(sizeStrings[1]);

            var vec = new Vector2(val1, val2);

            parser.MoveNext();

            return vec;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type) {
            throw new NotImplementedException();
        }

    }
}
