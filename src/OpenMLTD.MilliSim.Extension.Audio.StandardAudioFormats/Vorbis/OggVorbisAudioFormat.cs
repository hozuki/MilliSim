using System;
using JetBrains.Annotations;
using NAudio.Vorbis;
using NAudio.Wave;
using OpenMLTD.MilliSim.Audio;
using OpenMLTD.MilliSim.Plugin;

namespace OpenMLTD.MilliSim.Extension.Audio.StandardAudioFormats.Vorbis {
    [MilliSimPlugin(typeof(IAudioFormat))]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class OggVorbisAudioFormat : AudioFormat {

        public override string PluginID => "plugin.audio.ogg/vorbis";

        public override string PluginName => "Ogg/Vorbis Audio Format";

        public override string PluginDescription => "OGG audio format reader and factory.";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override WaveStream Read(string fileName) {
            return new VorbisWaveReader(fileName);
        }

        public override bool SupportsFileType(string fileName) {
            /* Ogg/Vorbis files are currently disabled.
             * MilliSim.Audio uses NAudio.Vorbis, thus NVorbis as its underlying decoding and streaming
             * library. At this point, if you try to seek the WaveStream when the last packets are consumed,
             * you will get a NullReferenceException from PacketReader.FindPacket() (in NVorbis). This
             * even happens when you try to seek to file start (0 sec). The final outcome is, during the
             * following playbacks (second time, third time, etc.), audio will be completely silent.
             * So I have to disable OGG support and hope users can accept only using MP3s.
             *
             * * NVorbis commit: https://github.com/ioctlLR/NVorbis/commit/478eb45c3996e45a6bd19777c0549b85a7bb6851
             * * NAudio.Vorbis commit: https://github.com/naudio/Vorbis/commit/0a16577fa9360304ae2a38d052955eb9e18e012d
             *
             * Temporarily solved by using a custom set of NVorbis and NAudio.Vorbis. Ogg/Vorbis support re-enabled.
             */

            fileName = fileName.ToLowerInvariant();
            return fileName.EndsWith(".ogg") || fileName.EndsWith(".oga") || fileName.EndsWith(".ogv");
        }

        public override string FormatDescription => "Ogg/Vorbis";

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
