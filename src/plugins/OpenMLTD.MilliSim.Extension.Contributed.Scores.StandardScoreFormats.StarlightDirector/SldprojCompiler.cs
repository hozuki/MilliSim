using OpenMLTD.MilliSim.Contributed.Scores;
using OpenMLTD.MilliSim.Contributed.Scores.Extending;
using OpenMLTD.MilliSim.Contributed.Scores.Runtime;
using OpenMLTD.MilliSim.Contributed.Scores.Source;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector {
    internal sealed class SldprojCompiler : DisposableBase, IScoreCompiler {

        public RuntimeScore Compile(SourceScore score, ScoreCompileOptions compileOptions) {
            compileOptions.Offset = (float)score.MusicOffset;
            return ScoreCompileHelper.CompileScore(score, compileOptions);
        }

        protected override void Dispose(bool disposing) {
        }

    }
}
