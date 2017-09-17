using System;
using System.Composition;
using System.IO;
using JetBrains.Annotations;
using NAudio.Wave;
using OpenMLTD.MilliSim.Audio.Extending;

namespace OpenMLTD.MilliSim.Extension.Imports.StandardAudioFormats.Wave {
    [Export(typeof(IAudioFormat))]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class WaveAudioFormat : IAudioFormat {

        public string PluginID => "plugin.audio.wave";

        public string PluginName => "Wave Audio Format";

        public string PluginDescription => "Wave audio format reader and factory.";

        public string PluginAuthor => "OpenMLTD";

        public Version PluginVersion => MyVersion;

        public WaveStream Read(string fileName) {
            return new WaveFileReader(fileName);
        }

        public bool SupportsFileType(string fileName) {
            fileName = fileName.ToLowerInvariant();
            return fileName.EndsWith(".wav");
        }

        public string FormatDescription => "Wave Audio";

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
