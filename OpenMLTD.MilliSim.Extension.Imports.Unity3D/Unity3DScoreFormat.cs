using System.Composition;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core.Entities.Extending;

namespace OpenMLTD.MilliSim.Extension.Imports.Unity3D {
    [Export(typeof(IScoreFormat))]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class Unity3DScoreFormat : IScoreFormat {

        public IScoreReader CreateReader() {
            return new Unity3DScoreReader();
        }

        public IScoreCompiler CreateCompiler() {
            return new Unity3DScoreCompiler();
        }

        public bool SupportsFileType(string fileName) {
            fileName = fileName.ToLowerInvariant();
            return fileName.EndsWith(".unity3d") || fileName.EndsWith(".unity3d.lz4");
        }

        public string Description => "Unity3D Score File";

    }
}
