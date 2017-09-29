using System;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Core.Entities.Extending;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Source;

namespace OpenMLTD.MilliSim.Extension.Scores.StandardScoreFormats.Mltd {
    internal sealed class Unity3DScoreCompiler : DisposableBase, IScoreCompiler {

        /// <summary>
        /// Compiles a <see cref="SourceScore"/> to a <see cref="RuntimeScore"/>, which will be used by the player.
        /// A <see cref="ScoreCompileOptions"/> object can be specified.
        /// </summary>
        /// <param name="score">The <see cref="SourceScore"/> to compile.</param>
        /// <param name="compileOptions">Compile options.</param>
        /// <returns>Compiled score.</returns>
        public RuntimeScore Compile(SourceScore score, ScoreCompileOptions compileOptions) {
            return ScoreCompileHelper.CompileScore(score, compileOptions);
        }

        protected override void Dispose(bool disposing) {
        }

        /**
         * A demonstration of the usage of ScoreCompilerHelper methods.
         */

        private RuntimeScore MyCompile(SourceScore score, ScoreCompileOptions compileOptions) {
            return ScoreCompileHelper.CompileScore(score, compileOptions, CreateNotes);
        }

        private static RuntimeNote[] CreateNotes(SourceNote note, Conductor[] conductors, SourceNote[] gameNotes, ref int currentID) {
            RuntimeNote[] notesToBeAdded;

            switch (note.Type) {
                case NoteType.Tap:
                    notesToBeAdded = MyCreateTapNote(note, conductors, ref currentID);
                    break;
                case NoteType.Flick:
                case NoteType.Hold:
                case NoteType.Slide:
                    notesToBeAdded = ScoreCompileHelper.CreateContinuousNotes(note, conductors, ref currentID);
                    break;
                case NoteType.Special:
                    notesToBeAdded = ScoreCompileHelper.CreateSpecialNotes(note, conductors, gameNotes, ref currentID);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return notesToBeAdded;
        }

        private static RuntimeNote[] MyCreateTapNote(SourceNote note, Conductor[] conductors, ref int currentID) {
            var rns = ScoreCompileHelper.CreateTapNote(note, conductors, ref currentID);
            if (note.ExtraInfo != null && note.ExtraInfo.ContainsKey("abc")) {
                var dyn = note.ExtraInfo.Clone();
                dyn.SetValue("def", 123);
                rns[0].ExtraInfo = dyn;
            }
            return rns;
        }

    }
}
