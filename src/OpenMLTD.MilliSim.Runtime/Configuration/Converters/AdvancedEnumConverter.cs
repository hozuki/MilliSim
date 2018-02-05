using System;
using JetBrains.Annotations;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Configuration.Converters {
    internal sealed class AdvancedEnumConverter : IYamlTypeConverter {

        internal AdvancedEnumConverter([NotNull] INamingConvention namingConvention) {
            _namingConvention = namingConvention;
        }

        public bool Accepts(Type type) {
            return type.IsEnum;
        }

        public object ReadYaml(IParser parser, Type type) {
            if (parser.Current == null) {
                return null;
            }

            var scalar = (Scalar)parser.Current;
            var str = scalar.Value;

            if (int.TryParse(str, out var i)) {
                return i;
            }

            str = _namingConvention.Apply(str);

            var result = Enum.Parse(type, str);

            parser.MoveNext();

            return result;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type) {
            throw new NotSupportedException();
        }

        private readonly INamingConvention _namingConvention;

    }
}
