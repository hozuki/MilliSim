using System;
using JetBrains.Annotations;
using NAudio.Wave;
using OpenMLTD.MilliSim.Audio;
using OpenMLTD.MilliSim.Audio.Extending;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Extension.Audio.StandardAudioFormats.Mpeg {
    [MilliSimPlugin(typeof(IAudioFormat))]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class Mp3AudioFormat : AudioFormat {

        public override string PluginID => "plugin.audio.mp3";

        public override string PluginName => "MP3 Audio Format";

        public override string PluginDescription => "MP3 audio format reader and factory.";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override WaveStream Read(string fileName) {
            return new Mp3FileReader(fileName);
        }

        public override bool SupportsFileType(string fileName) {
            fileName = fileName.ToLowerInvariant();
            return fileName.EndsWith(".mp3");
        }

        public override string FormatDescription => "MPEG Layer 3";

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
