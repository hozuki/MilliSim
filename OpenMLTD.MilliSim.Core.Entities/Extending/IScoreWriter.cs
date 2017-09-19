using System;
using System.IO;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Source;

namespace OpenMLTD.MilliSim.Core.Entities.Extending {
    public interface IScoreWriter : IDisposable {

        void WriteSourceScore([NotNull] Stream stream, [NotNull] SourceScore score);

        void WriteCompiledScore([NotNull] Stream stream, [NotNull] RuntimeScore score);

    }
}
