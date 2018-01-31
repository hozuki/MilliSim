using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Glob;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Globalization {
    public sealed class TranslationManager {

        internal TranslationManager([NotNull] CultureSpecificInfo cultureSpecificInfo) {
            CultureSpecificInfo = cultureSpecificInfo;
        }

        public void AddTranslationsFromGlob([NotNull] string baseDirectory, [NotNull] string pattern) {
            var directoryInfo = new DirectoryInfo(baseDirectory);

            AddTranslationsFromGlob(directoryInfo, pattern);
        }

        public void AddTranslationsFromGlob([NotNull] DirectoryInfo baseDirectory, [NotNull] string pattern) {
            var baseDirectoryFullPath = baseDirectory.FullName;
            var globCharIndex = pattern.IndexOfAny(GlobHelper.PartialGlobChars);

            if (globCharIndex < 0) {
                string fullFileName;

                if (Path.IsPathRooted(pattern)) {
                    fullFileName = pattern;
                } else {
                    fullFileName = Path.Combine(baseDirectoryFullPath, pattern);
                }

                AddTranslationsFromFile(Path.Combine(baseDirectoryFullPath, fullFileName));

                return;
            }

            string globString;
            DirectoryInfo globDirectory;

            if (globCharIndex == 0) {
                globString = pattern;
                globDirectory = baseDirectory;
            } else {
                var lastSpeparator = pattern.LastIndexOfAny(GlobHelper.PathSeparatorChars, globCharIndex - 1);
                var subPath = pattern.Substring(0, lastSpeparator);

                globDirectory = new DirectoryInfo(Path.Combine(baseDirectoryFullPath, subPath));
                globString = pattern.Substring(globCharIndex);
            }

            if (globDirectory.Exists) {
                foreach (var file in globDirectory.GlobFiles(globString)) {
                    AddTranslationsFromFile(file.FullName);
                }
            }
        }

        public void AddTranslationsFromFile([NotNull] string translationFilePath) {
            AddTranslationsFromFile(translationFilePath, Encoding.UTF8);
        }

        public void AddTranslationsFromFile([NotNull] string translationFilePath, [NotNull] Encoding encoding) {
            var fileInfo = new FileInfo(translationFilePath);
            var culture = CultureSpecificInfo.Culture;
            var candidates = new List<string>();

            var dirName = fileInfo.DirectoryName;

            if (dirName == null) {
                throw new ApplicationException("Why oh why the directory name is null?");
            }

            // Notice the order. The latter ones will overwrite the former ones.
            const string fileNameFormat = "{0}.{1}.{2}";
            candidates.Add(fileInfo.FullName);

            var fileExtension = fileInfo.Extension;
            // 4 = ".iv.".length
            var fileBaseName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileExtension.Length - 4);

            candidates.Add(Path.Combine(dirName, string.Format(fileNameFormat, fileBaseName, culture.ThreeLetterISOLanguageName, fileExtension)));
            candidates.Add(Path.Combine(dirName, string.Format(fileNameFormat, fileBaseName, culture.TwoLetterISOLanguageName, fileExtension)));
            candidates.Add(Path.Combine(dirName, string.Format(fileNameFormat, fileBaseName, culture.ThreeLetterWindowsLanguageName, fileExtension)));
            candidates.Add(Path.Combine(dirName, string.Format(fileNameFormat, fileBaseName, culture.LCID, fileExtension)));
            candidates.Add(Path.Combine(dirName, string.Format(fileNameFormat, fileBaseName, culture.Name, fileExtension)));

            foreach (var fileName in candidates) {
                if (File.Exists(fileName)) {
                    AddTranslationsFromExactFile(fileName, encoding);
                }
            }
        }

        public void AddTranslationsFromExactFile([NotNull] string translationFilePath, [NotNull] Encoding encoding) {
            var des = new Deserializer();
            IReadOnlyDictionary<object, object> obj;

            using (var fileStream = File.Open(translationFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var reader = new StreamReader(fileStream, encoding)) {
                    obj = (IReadOnlyDictionary<object, object>)des.Deserialize(reader);
                }
            }

            var q = new Queue<(string Prefix, object Object)>();
            q.Enqueue((string.Empty, obj));

            var translations = _translations;
            while (q.Count > 0) {
                var o = q.Dequeue();

                if (o.Object is IReadOnlyDictionary<object, object> dict) {
                    var higherPrefix = o.Prefix.Length > 0 ? o.Prefix + LevelDelimeter : string.Empty;

                    foreach (var kv in dict) {
                        q.Enqueue((Prefix: higherPrefix + kv.Key, Object: kv.Value));
                    }
                } else if (o.Object is IReadOnlyList<object>) {
                    throw new FormatException("You cannot use a list in translation file.");
                } else {
                    translations[o.Prefix] = o.Object.ToString();
                }
            }
        }

        public void ClearTranslations() {
            _translations.Clear();
        }

        [NotNull]
        public string Get([NotNull] string key) {
            return _translations[key];
        }

        public bool ContainsKey([NotNull] string key) {
            return _translations.ContainsKey(key);
        }

        internal CultureSpecificInfo CultureSpecificInfo { get; }

        private static readonly string LevelDelimeter = ".";
        private readonly Dictionary<string, string> _translations = new Dictionary<string, string>();

    }
}
