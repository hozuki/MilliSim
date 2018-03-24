using System;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Editor {
    public sealed class ProjectSettings : ICloneable, ICloneable<ProjectSettings> {

        public static readonly double DefaultBeatPerMinute = 120;
        public static readonly double DefaultStartTimeOffset = 0;
        public static readonly int DefaultGridPerSignature = 24;
        public static readonly int DefaultSignature = 4;

        /// <summary>
        /// Tempo，每分钟四分音符出现次数。
        /// </summary>
        public double BeatPerMinute { get; set; }

        public double StartTimeOffset { get; set; }

        /// <summary>
        /// 细分级别，一个四分音符被分成多少份。
        /// 例如，分成2份，拍号3（3/4拍），速度120，则每一个小节长度为1.5秒（=60÷120×3），每个note定位精度为一个八分音符（=1/2四分音符）。
        /// </summary>
        public int GridPerSignature { get; set; }

        /// <summary>
        /// 拍号，以四分音符为标准，即 x/4 拍。
        /// </summary>
        public int Signature { get; set; }

        public static ProjectSettings CreateDefault() {
            return new ProjectSettings {
                BeatPerMinute = DefaultBeatPerMinute,
                StartTimeOffset = DefaultStartTimeOffset,
                GridPerSignature = DefaultGridPerSignature, // 最高分辨率为九十六分音符
                Signature = DefaultSignature // 4/4拍
            };
        }

        public ProjectSettings Clone() {
            return new ProjectSettings {
                BeatPerMinute = BeatPerMinute,
                StartTimeOffset = StartTimeOffset,
                GridPerSignature = GridPerSignature,
                Signature = Signature
            };
        }

        private ProjectSettings() {
        }

        object ICloneable.Clone() {
            return Clone();
        }

    }
}
