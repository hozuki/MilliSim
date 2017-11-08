using System;
using JetBrains.Annotations;
using NAudio.Wave;
using OpenMLTD.MilliSim.Audio.Extending;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Extension.Audio.StandardAudioFormats.Wave {
    [MilliSimPlugin(typeof(IAudioFormat))]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class WaveAudioFormat : AudioFormat {

        public override string PluginID => "plugin.audio.wave";

        public override string PluginName => "Wave Audio Format";

        public override string PluginDescription => "Wave audio format reader and factory.";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override WaveStream Read(string fileName) {
            return new WaveFileReader(fileName);
        }

        public override bool SupportsFileType(string fileName) {
            fileName = fileName.ToLowerInvariant();
            return fileName.EndsWith(".wav");
        }

        public override string FormatDescription => "Wave Audio";

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
