using System;
using System.IO;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core.Entities.Extending {
    public interface IScoreReader : IDisposable {

        [NotNull]
        Score Read([NotNull] Stream stream, [NotNull] string fileName, [NotNull] IFlexibleOptions options);

        bool TryRead([NotNull] Stream stream, [NotNull] string fileName, [NotNull] IFlexibleOptions options, [CanBeNull] out Score score);

    }
}
