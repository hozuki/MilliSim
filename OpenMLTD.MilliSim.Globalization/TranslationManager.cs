using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Globalization {
    public sealed class TranslationManager {

        internal TranslationManager([NotNull] CultureSpecificInfo cultureSpecificInfo) {
            CultureSpecificInfo = cultureSpecificInfo;
        }

        internal CultureSpecificInfo CultureSpecificInfo { get; }

        public void AddTranslationsFromFile([NotNull] string translationFilePath) {
            AddTranslationsFromFile(translationFilePath, Encoding.UTF8);
        }

        public void AddTranslationsFromFile([NotNull] string translationFilePath, [NotNull] Encoding encoding) {
            var fileInfo = new FileInfo(translationFilePath);
            var culture = CultureSpecificInfo.Culture;
            var candidates = new List<string>();

            var dirName = fileInfo.DirectoryName;

            // Notice the order. The one appearing later will overwrite the one appearing earlier.
            const string fileNameFormat = "{0}.{1}.mui";
            candidates.Add(fileInfo.FullName);
            candidates.Add(Path.Combine(dirName, string.Format(fileNameFormat, fileInfo.Name, culture.ThreeLetterISOLanguageName)));
            candidates.Add(Path.Combine(dirName, string.Format(fileNameFormat, fileInfo.Name, culture.TwoLetterISOLanguageName)));
            candidates.Add(Path.Combine(dirName, string.Format(fileNameFormat, fileInfo.Name, culture.ThreeLetterWindowsLanguageName)));
            candidates.Add(Path.Combine(dirName, string.Format(fileNameFormat, fileInfo.Name, culture.LCID)));
            candidates.Add(Path.Combine(dirName, string.Format(fileNameFormat, fileInfo.Name, culture.Name)));

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

        private static readonly string LevelDelimeter = ".";
        private readonly Dictionary<string, string> _translations = new Dictionary<string, string>();

    }
}
