using System;
using System.IO;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Contributed.Scores.Runtime;
using OpenMLTD.MilliSim.Contributed.Scores.Source;

namespace OpenMLTD.MilliSim.Contributed.Scores.Extending {
    public interface IScoreReader : IDisposable {

        bool IsStreamingSupported { get; }

        [NotNull]
        SourceScore ReadSourceScore([CanBeNull] Stream stream, [NotNull] string fileName, [NotNull] ReadSourceOptions sourceOptions);

        [NotNull]
        RuntimeScore ReadCompiledScore([CanBeNull] Stream stream, [NotNull] string fileName, [NotNull] ReadSourceOptions sourceOptions, [NotNull] ScoreCompileOptions compileOptions);

    }
}
