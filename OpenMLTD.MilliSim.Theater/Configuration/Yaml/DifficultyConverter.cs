using System;
using OpenMLTD.MilliSim.Core.Entities;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Theater.Configuration.Yaml {
    public sealed class DifficultyConverter : IYamlTypeConverter {

        public bool Accepts(Type type) {
            return type == typeof(Difficulty);
        }

        public object ReadYaml(IParser parser, Type type) {
            if (parser.Current == null) {
                return null;
            }
            var scalar = (Scalar)parser.Current;
            var str = scalar.Value.Trim().ToLowerInvariant();
            Difficulty ret;
            switch (str) {
                case "2m":
                    ret = Difficulty.D2Mix;
                    break;
                case "2m+":
                    ret = Difficulty.D2MixPlus;
                    break;
                case "4m":
                    ret = Difficulty.D4Mix;
                    break;
                case "6m":
                    ret = Difficulty.D6Mix;
                    break;
                case "mm":
                    ret = Difficulty.MillionMix;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(str), str, null);
            }
            parser.MoveNext();
            return ret;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type) {
            throw new NotImplementedException();
        }

    }
}
