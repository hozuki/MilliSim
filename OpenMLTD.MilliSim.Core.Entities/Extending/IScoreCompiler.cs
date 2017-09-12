using System;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core.Entities.Runtime;

namespace OpenMLTD.MilliSim.Core.Entities.Extending {
    public interface IScoreCompiler : IDisposable {

        RuntimeScore Compile([NotNull] Score score, [NotNull] IFlexibleOptions options);

    }
}
