using System;
using System.IO;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Contributed.Scores.Runtime;
using OpenMLTD.MilliSim.Contributed.Scores.Source;

namespace OpenMLTD.MilliSim.Plugin {
    public interface IScoreReader : IDisposable {

        [NotNull]
        SourceScore ReadSourceScore([NotNull] Stream stream, [NotNull] string fileName, [NotNull] ReadSourceOptions sourceOptions);

        [NotNull]
        RuntimeScore ReadCompiledScore([NotNull] Stream stream, [NotNull] string fileName, [NotNull] ReadSourceOptions sourceOptions, [NotNull] ScoreCompileOptions compileOptions);

    }
}
