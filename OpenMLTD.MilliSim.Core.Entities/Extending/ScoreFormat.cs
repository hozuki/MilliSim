using System;

namespace OpenMLTD.MilliSim.Core.Entities.Extending {
    public abstract class ScoreFormat : IScoreFormat {

        public abstract string PluginID { get; }

        public abstract string PluginName { get; }

        public abstract string PluginDescription { get; }

        public abstract string PluginAuthor { get; }

        public abstract Version PluginVersion { get; }

        public string PluginCategory => "Score";

        public abstract IScoreReader CreateReader();

        public abstract IScoreCompiler CreateCompiler();

        public abstract bool SupportsFileType(string fileName);

        public abstract string FormatDescription { get; }

        public int ApiVersion => 1;

    }
}
