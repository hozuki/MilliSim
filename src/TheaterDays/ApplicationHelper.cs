using System;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.TheaterDays {
    internal static class ApplicationHelper {

        [NotNull]
        internal static string StartupPath => LazyStartupPath.Value;

        [NotNull]
        internal static string CodeName => LazyCodeName.Value;

        [NotNull]
        private static string GetStartupPath() {
            var assembly = Assembly.GetEntryAssembly();
            var codeBaseUri = new Uri(assembly.EscapedCodeBase);
            var assemblyFilePath = codeBaseUri.LocalPath;
            var fileInfo = new FileInfo(assemblyFilePath);

            return fileInfo.DirectoryName ?? Environment.CurrentDirectory;
        }

        [CanBeNull]
        private static string GetCodeName() {
            var assembly = Assembly.GetEntryAssembly();
            var attr = assembly.GetCustomAttribute<MilliSimCodeNameAttribute>();

            return attr?.CodeName;
        }

        [NotNull]
        private static readonly Lazy<string> LazyStartupPath = new Lazy<string>(GetStartupPath);

        [NotNull]
        private static readonly Lazy<string> LazyCodeName = new Lazy<string>(GetCodeName);

    }
}
