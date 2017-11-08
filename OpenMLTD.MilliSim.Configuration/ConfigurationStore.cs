using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Glob;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Configuration.Entities;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Configuration {
    public sealed class ConfigurationStore : IReadOnlyDictionary<Type, ConfigBase> {

        private ConfigurationStore() {
        }

        /// <summary>
        /// Loads and fills a <see cref="ConfigurationStore"/> using UTF-8 encoding.
        /// </summary>
        /// <param name="path">Path of the entry file.</param>
        /// <param name="deserializer">The deserializer to deserialize files. It must support <see cref="DeserializerBuilder.IgnoreUnmatchedProperties"/>.</param>
        /// <returns></returns>
        public static ConfigurationStore Load([NotNull] string path, [NotNull] Deserializer deserializer) {
            return Load(path, deserializer, Encoding.UTF8);
        }

        /// <summary>
        /// Loads and fills a <see cref="ConfigurationStore"/>.
        /// </summary>
        /// <param name="path">Path of the entry file.</param>
        /// <param name="deserializer">The deserializer to deserialize files. It must support <see cref="DeserializerBuilder.IgnoreUnmatchedProperties"/>.</param>
        /// <param name="encoding">File encoding.</param>
        /// <returns></returns>
        public static ConfigurationStore Load([NotNull] string path, [NotNull] Deserializer deserializer, [NotNull] Encoding encoding) {
            var configFileList = ScanConfigFiles(path, encoding);

            var store = new ConfigurationStore();
            var dict = store._configurations;

            foreach (var p in configFileList) {
                using (var fileStream = File.Open(p, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    using (var reader = new StreamReader(fileStream, encoding)) {
                        var baseObj = deserializer.Deserialize<HeaderOnlyConfig>(reader);

                        var assemblyFile = baseObj.Metadata.AssemblyFile;
                        var assembly = Assembly.LoadFrom(assemblyFile);

                        var desiredTypeName = baseObj.Metadata.Type;
                        var qualifiedName = Assembly.CreateQualifiedName(assembly.FullName, desiredTypeName);
                        var desiredType = Type.GetType(qualifiedName, false, true);

                        if (desiredType == null) {
                            throw new TypeAccessException($"Desired type '{desiredTypeName}' does not exist.");
                        }

                        if (dict.ContainsKey(desiredType)) {
                            throw new DuplicateKeyException($"Config object '{desiredTypeName}' already exists.");
                        }

                        reader.BaseStream.Position = 0;

                        var obj = deserializer.Deserialize(reader, desiredType);
                        var obj2 = (ConfigBase)obj;
                        dict[desiredType] = obj2;
                    }
                }
            }

            return store;
        }

        public IEnumerator<KeyValuePair<Type, ConfigBase>> GetEnumerator() {
            return _configurations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public int Count => _configurations.Count;

        public bool ContainsKey(Type key) {
            return _configurations.ContainsKey(key);
        }

        public bool TryGetValue(Type key, out ConfigBase value) {
            return _configurations.TryGetValue(key, out value);
        }

        public ConfigBase this[Type key] => _configurations[key];

        public T Get<T>() where T : ConfigBase {
            var t = typeof(T);
            return (T)_configurations[t];
        }

        public ConfigBase Get([NotNull] Type type) {
            if (!type.IsSubclassOf(typeof(ConfigBase))) {
                throw new ArgumentException();
            }
            return _configurations[type];
        }

        public IEnumerable<Type> Keys => _configurations.Keys;

        public IEnumerable<ConfigBase> Values => _configurations.Values;

        private static IReadOnlyList<string> ScanConfigFiles([NotNull] string entryFilePath, [NotNull] Encoding encoding) {
            var result = new List<string>();
            var q = new Queue<(string Path, string BasePath)>();
            entryFilePath = Path.GetFullPath(entryFilePath);
            q.Enqueue((Path: Path.GetFileName(entryFilePath), BasePath: Path.GetDirectoryName(entryFilePath)));

            while (q.Count > 0) {
                var (path, basePath) = q.Dequeue();

                if (!Path.IsPathRooted(path)) {
                    // Treat as absolute path.
                    path = Path.Combine(basePath, path);
                }

                var globCharIndex = path.IndexOfAny(PartialGlobChars);
                if (globCharIndex == 0) {
                    throw new ArgumentOutOfRangeException(nameof(globCharIndex));
                }

                if (globCharIndex >= 0) {
                    var sepIndex = path.LastIndexOfAny(Separators, globCharIndex - 1);
                    if (sepIndex < 0) {
                        throw new ArgumentOutOfRangeException(nameof(sepIndex));
                    }

                    basePath = path.Substring(0, sepIndex + 1);

                    var dirInfo = new DirectoryInfo(basePath);
                    var subPath = path.Substring(sepIndex + 1);

                    foreach (var fileInfo in dirInfo.GlobFiles(subPath)) {
                        if (result.Contains(fileInfo.FullName)) {
                            continue;
                        }

                        q.Enqueue((Path: fileInfo.Name, BasePath: fileInfo.DirectoryName));
                    }

                    continue;
                } else {
                    if (!result.Contains(path)) {
                        result.Add(path);
                    }
                }

                using (var fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    using (var streamReader = new StreamReader(fileStream, encoding)) {
                        // Reads until the first line which:
                        // - is not blank;
                        // - does not start with "#", or
                        //   the content inside the comments does not start with "@include"
                        while (!streamReader.EndOfStream) {
                            var line = streamReader.ReadLine();
                            if (line == null) {
                                break;
                            }

                            if (string.IsNullOrWhiteSpace(line)) {
                                continue;
                            }

                            line = line.TrimStart();
                            if (!line.StartsWith("#")) {
                                break;
                            }

                            line = line.Substring(1).Trim();
                            var match = IncludeRegex.Match(line);
                            if (!match.Success) {
                                break;
                            }

                            var includePath = match.Groups["path"].Value;
                            if (string.IsNullOrEmpty(includePath)) {
                                continue;
                            }

                            q.Enqueue((Path: includePath, BasePath: basePath));
                        }
                    }
                }
            }

            return result;
        }

        private readonly Dictionary<Type, ConfigBase> _configurations = new Dictionary<Type, ConfigBase>();
        private static readonly char[] PartialGlobChars = { '*', '?' };
        private static readonly char[] Separators = { '\\', '/' };
        private static readonly Regex IncludeRegex = new Regex(@"^@include\s+\<(?<path>[^>]*)\>$");

    }
}
