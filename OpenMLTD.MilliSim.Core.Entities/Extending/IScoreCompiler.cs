using System;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Source;

namespace OpenMLTD.MilliSim.Core.Entities.Extending {
    public interface IScoreCompiler : IDisposable {

        RuntimeScore Compile([NotNull] SourceScore score, [NotNull] ScoreCompileOptions options);

    }
}
