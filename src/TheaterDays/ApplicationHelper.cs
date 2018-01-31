using System;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;

namespace OpenMLTD.TheaterDays {
    internal static class ApplicationHelper {

        [NotNull]
        internal static string StartupPath => LazyStartupPath.Value;

        [NotNull]
        private static string GetStartupPath() {
            var assembly = Assembly.GetExecutingAssembly();
            var codeBaseUri = new Uri(assembly.EscapedCodeBase);
            var assemblyFilePath = codeBaseUri.LocalPath;
            var fileInfo = new FileInfo(assemblyFilePath);

            return fileInfo.DirectoryName ?? Environment.CurrentDirectory;
        }

        [NotNull]
        private static readonly Lazy<string> LazyStartupPath = new Lazy<string>(GetStartupPath);

    }
}
