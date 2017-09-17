using System;
using System.Composition;
using JetBrains.Annotations;
using NAudio.Wave;
using OpenMLTD.MilliSim.Audio.Extending;

namespace OpenMLTD.MilliSim.Extension.Imports.StandardAudioFormats.Mpeg {
    [Export(typeof(IAudioFormat))]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class Mp3AudioFormat : IAudioFormat {

        public string PluginID => "plugin.audio.mp3";

        public string PluginName => "MP3 Audio Format";

        public string PluginDescription => "MP3 audio format reader and factory.";

        public string PluginAuthor => "OpenMLTD";

        public Version PluginVersion => MyVersion;

        public WaveStream Read(string fileName) {
            return new Mp3FileReader(fileName);
        }

        public bool SupportsFileType(string fileName) {
            fileName = fileName.ToLowerInvariant();
            return fileName.EndsWith(".mp3");
        }

        public string FormatDescription => "MPEG Layer 3";

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
