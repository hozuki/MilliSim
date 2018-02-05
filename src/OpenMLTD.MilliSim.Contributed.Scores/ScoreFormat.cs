using System;
using System.Collections.Generic;
using OpenMLTD.MilliSim.Contributed.Scores.Extending;

namespace OpenMLTD.MilliSim.Contributed.Scores {
    /// <inheritdoc cref="IScoreFormat"/>
    /// <summary>
    /// An abstract score format.
    /// </summary>
    public abstract class ScoreFormat : IScoreFormat {

        public abstract string PluginID { get; }

        public abstract string PluginName { get; }

        public abstract string PluginDescription { get; }

        public abstract string PluginAuthor { get; }

        public abstract Version PluginVersion { get; }

        public string PluginCategory => "Score";

        public abstract bool CanReadAsSource { get; }

        public abstract bool CanReadAsCompiled { get; }

        public abstract bool CanBeCompiled { get; }

        public abstract bool CanWriteSource { get; }

        public abstract bool CanWriteCompiled { get; }

        public abstract IScoreReader CreateReader();

        public abstract IScoreWriter CreateWriter();

        public abstract IScoreCompiler CreateCompiler();

        public abstract bool SupportsReadingFileType(string fileName);

        public abstract IReadOnlyList<string> SupportedReadExtensions { get; }

        public abstract IReadOnlyList<string> SupportedWriteExtensions { get; }

        public abstract string FormatDescription { get; }

        public int ApiVersion => 1;

    }
}
