using System;
using System.IO;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities.Extending;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Source;

namespace OpenMLTD.MilliSim.Extension.Scores.StandardScoreFormats.Mltd {
    internal sealed class Unity3DScoreWriter : DisposableBase, IScoreWriter {

        public void WriteSourceScore(Stream stream, SourceScore score) {
            throw new NotSupportedException();
        }

        public void WriteCompiledScore(Stream stream, RuntimeScore score) {
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing) {
        }

    }
}
