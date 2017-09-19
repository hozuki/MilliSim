using System;
using System.Composition;
using JetBrains.Annotations;
using NAudio.Flac;
using NAudio.Wave;
using OpenMLTD.MilliSim.Audio.Extending;

namespace OpenMLTD.MilliSim.Extension.Audio.StandardAudioFormats.Flac {
    [Export(typeof(IAudioFormat))]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class FlacAudioFormat : AudioFormat {

        public override string PluginID => "plugin.audio.flac";

        public override string PluginName => "FLAC Audio Format";

        public override string PluginDescription => "FLAC audio format reader and factory.";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override WaveStream Read(string fileName) {
            return new FlacReader(fileName);
        }

        public override bool SupportsFileType(string fileName) {
            fileName = fileName.ToLowerInvariant();
            return fileName.EndsWith(".flac");
        }

        public override string FormatDescription => "FLAC Audio";

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
