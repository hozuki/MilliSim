using System;
using System.IO;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Contributed.Scores.Runtime;
using OpenMLTD.MilliSim.Contributed.Scores.Source;

namespace OpenMLTD.MilliSim.Plugin {
    public interface IScoreWriter : IDisposable {

        void WriteSourceScore([NotNull] Stream stream, [NotNull] string fileName, [NotNull] SourceScore score);

        void WriteCompiledScore([NotNull] Stream stream, [NotNull] string fileName, [NotNull] RuntimeScore score);

    }
}
