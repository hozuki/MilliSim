using System;
using System.Composition;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core.Entities.Extending;

namespace OpenMLTD.MilliSim.Extension.Imports.Unity3D {
    [Export(typeof(IScoreFormat))]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class Unity3DScoreFormat : IScoreFormat {

        public string PluginID => "plugin.score.unity3d";

        public string PluginName => "Unit3D Score Format";

        public string PluginDescription => "Unity3D score format reader and compiler factory.";

        public string PluginAuthor => "OpenMLTD";

        public Version PluginVersion => MyVersion;

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

        public string FormatDescription => "Unity3D Score File";

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
