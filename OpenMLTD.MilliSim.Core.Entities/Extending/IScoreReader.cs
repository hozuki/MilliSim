using JetBrains.Annotations;
using System;
using System.IO;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Source;

namespace OpenMLTD.MilliSim.Core.Entities.Extending {
    public interface IScoreReader : IDisposable {

        [NotNull]
        SourceScore ReadSourceScore([NotNull] Stream stream, [NotNull] string fileName, [NotNull] ReadSourceOptions options);

        [NotNull]
        RuntimeScore ReadCompiledScore([NotNull] Stream stream, [NotNull] string fileName, [NotNull] ReadSourceOptions sourceOptions, ScoreCompileOptions compileOptions);

    }
}
