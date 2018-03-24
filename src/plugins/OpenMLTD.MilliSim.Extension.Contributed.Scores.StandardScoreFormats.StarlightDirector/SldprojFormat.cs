using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Contributed.Scores;
using OpenMLTD.MilliSim.Contributed.Scores.Extending;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector {
    [MilliSimPlugin(typeof(IScoreFormat))]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class SldprojFormat : ScoreFormat {

        public override string PluginID => "plugin.score.starlight_director";

        public override string PluginName => "Starlight Director project format (target: CGSS)";

        public override string PluginDescription => "Reader and writer factory for Starlight Director project format.";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override bool CanReadAsSource => true;

        public override bool CanReadAsCompiled => true;

        public override bool CanBeCompiled => true;

        public override bool CanWriteSource => false;

        public override bool CanWriteCompiled => false;

        public override IScoreReader CreateReader() {
            return new SldprojReader();
        }

        public override IScoreWriter CreateWriter() {
            return new SldprojWriter();
        }

        public override IScoreCompiler CreateCompiler() {
            return new SldprojCompiler();
        }

        public override bool SupportsReadingFileType(string fileName) {
            fileName = fileName.ToLowerInvariant();
            return SupportedReadExtensions.Any(ext => fileName.EndsWith(ext));
        }

        public override IReadOnlyList<string> SupportedReadExtensions => SldprojFileReadExtensions;

        public override IReadOnlyList<string> SupportedWriteExtensions => SldprojFileWriteExtensions;

        public override string FormatDescription => "Starlight Director project";

        private static readonly string[] SldprojFileReadExtensions = { ".sldproj" };
        private static readonly string[] SldprojFileWriteExtensions = new string[0];

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
