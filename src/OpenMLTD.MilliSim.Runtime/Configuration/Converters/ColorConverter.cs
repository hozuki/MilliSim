using System;
using Microsoft.Xna.Framework;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Configuration.Converters {
    public sealed class ColorConverter : IYamlTypeConverter {

        public bool Accepts(Type type) {
            return type == typeof(Color);
        }

        public object ReadYaml(IParser parser, Type type) {
            if (parser.Current == null) {
                return null;
            }

            var scalar = (Scalar)parser.Current;
            var str = scalar.Value;
            if (string.IsNullOrWhiteSpace(str)) {
                return default(Color);
            }

            str = str.Trim().ToLowerInvariant();

            if (str.StartsWith("#")) {
                str = str.TrimStart('#');
            }

            if (str.Length < 8) {
                if (str.Length < 6) {
                    str = str.PadLeft(6, '0');
                }
                // Become solid.
                str = str.PadLeft(8, 'f');
            }

            var val = Convert.ToInt32(str, 16);

            var a = (val >> 24) & 0xff;
            var r = (val >> 16) & 0xff;
            var g = (val >> 8) & 0xff;
            var b = val & 0xff;

            var color = Color.FromNonPremultiplied(r, g, b, a);

            parser.MoveNext();

            return color;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type) {
            throw new NotImplementedException();
        }

    }
}
