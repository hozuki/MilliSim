using System;
using System.Composition;
using JetBrains.Annotations;
using NAudio.Wave;
using NAudio.WindowsMediaFormat;
using OpenMLTD.MilliSim.Audio.Extending;

namespace OpenMLTD.MilliSim.Extension.Imports.StandardAudioFormats.Wma {
    [Export(typeof(IAudioFormat))]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class WmaAudioFormat : AudioFormat {

        public override string PluginID => "plugin.audio.wma";

        public override string PluginName => "WMA Audio Format";

        public override string PluginDescription => "WMA audio format reader and factory.";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override WaveStream Read(string fileName) {
            return new WMAFileReader(fileName);
        }

        public override bool SupportsFileType(string fileName) {
            fileName = fileName.ToLowerInvariant();
            return fileName.EndsWith(".wma");
        }

        public override string FormatDescription => "Windows Media Audio";

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
