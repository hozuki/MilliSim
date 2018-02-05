using System;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Contributed.Scores.Runtime;
using OpenMLTD.MilliSim.Contributed.Scores.Source;

namespace OpenMLTD.MilliSim.Contributed.Scores.Extending {
    public interface IScoreCompiler : IDisposable {

        RuntimeScore Compile([NotNull] SourceScore score, [NotNull] ScoreCompileOptions compileOptions);

    }
}
