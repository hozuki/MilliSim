using JetBrains.Annotations;
using NAudio.Wave;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Audio.Extending {
    public interface IAudioFormat : IMilliSimPlugin {

        WaveStream Read([NotNull] string fileName);

        bool SupportsFileType([NotNull] string fileName);

        string FormatDescription { get; }

    }
}
