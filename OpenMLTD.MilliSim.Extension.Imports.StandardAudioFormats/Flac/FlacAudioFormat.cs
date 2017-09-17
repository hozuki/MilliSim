using System;
using System.Composition;
using JetBrains.Annotations;
using NAudio.Flac;
using NAudio.Wave;
using OpenMLTD.MilliSim.Audio.Extending;

namespace OpenMLTD.MilliSim.Extension.Imports.StandardAudioFormats.Flac {
    [Export(typeof(IAudioFormat))]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class FlacAudioFormat : IAudioFormat {

        public string PluginID => "plugin.audio.flac";

        public string PluginName => "FLAC Audio Format";

        public string PluginDescription => "FLAC audio format reader and factory.";

        public string PluginAuthor => "OpenMLTD";

        public Version PluginVersion => MyVersion;

        public WaveStream Read(string fileName) {
            return new FlacReader(fileName);
        }

        public bool SupportsFileType(string fileName) {
            fileName = fileName.ToLowerInvariant();
            return fileName.EndsWith(".flac");
        }

        public string FormatDescription => "FLAC Audio";

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
