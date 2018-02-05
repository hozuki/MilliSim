using System;
using System.IO;
using OpenMLTD.MilliSim.Contributed.Scores.Extending;
using OpenMLTD.MilliSim.Contributed.Scores.Runtime;
using OpenMLTD.MilliSim.Contributed.Scores.Source;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.Mltd {
    internal sealed class Unity3DScoreWriter : DisposableBase, IScoreWriter {

        public void WriteSourceScore(Stream stream, string fileName, SourceScore score) {
            throw new NotSupportedException();
        }

        public void WriteCompiledScore(Stream stream, string fileName, RuntimeScore score) {
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing) {
        }

    }
}
