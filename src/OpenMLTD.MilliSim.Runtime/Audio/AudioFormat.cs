using System;
using NAudio.Wave;
using OpenMLTD.MilliSim.Audio.Extending;

namespace OpenMLTD.MilliSim.Audio {
    /// <inheritdoc />
    /// <summary>
    /// A basic implementation of <see cref="T:OpenMLTD.MilliSim.Audio.Extending.IAudioFormat" />.
    /// </summary>
    public abstract class AudioFormat : IAudioFormat {

        public abstract string PluginID { get; }

        public abstract string PluginName { get; }

        public abstract string PluginDescription { get; }

        public abstract string PluginAuthor { get; }

        public abstract Version PluginVersion { get; }

        public string PluginCategory => "Audio Format";

        public abstract WaveStream Read(string fileName);

        public abstract bool SupportsFileType(string fileName);

        public abstract string FormatDescription { get; }

        public int ApiVersion => 1;

    }
}
