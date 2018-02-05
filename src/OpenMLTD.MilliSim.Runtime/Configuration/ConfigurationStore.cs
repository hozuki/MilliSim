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
using OpenMLTD.MilliSim.Core;
using YamlDotNet.Serialization;

namespace OpenMLTD.MilliSim.Configuration {
    public sealed class ConfigurationStore : IReadOnlyDictionary<Type, ConfigBase> {

        private ConfigurationStore() {
        }

        /// <summary>
        /// Loads and fills a <see cref="ConfigurationStore"/> using UTF-8 encoding.
        /// </summary>
        /// <param name="entryFilePath">Path of the entry file.</param>
        /// <param name="deserializer">The deserializer to deserialize files. It must support <see cref="DeserializerBuilder.IgnoreUnmatchedProperties"/>.</param>
        /// <returns></returns>
        public static ConfigurationStore Load([NotNull] string entryFilePath, [NotNull] Deserializer deserializer) {
            return Load(entryFilePath, deserializer, Encoding.UTF8);
        }

        /// <summary>
        /// Loads and fills a <see cref="ConfigurationStore"/>.
        /// </summary>
        /// <param name="entryFilePath">Path of the entry file.</param>
        /// <param name="deserializer">The deserializer to deserialize files. It must support <see cref="DeserializerBuilder.IgnoreUnmatchedProperties"/>.</param>
        /// <param name="encoding">File encoding.</param>
        /// <returns>Created <see cref="ConfigurationStore"/>.</returns>
        public static ConfigurationStore Load([NotNull] string entryFilePath, [NotNull] Deserializer deserializer, [NotNull] Encoding encoding) {
            var configFileList = ScanConfigFiles(entryFilePath, encoding);

            var store = new ConfigurationStore();
            var dict = store._configurations;

            foreach (var p in configFileList) {
                using (var fileStream = File.Open(p, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    using (var reader = new StreamReader(fileStream, encoding)) {
                        var baseObj = deserializer.Deserialize<HeaderOnlyConfig>(reader);

                        var assemblyFile = baseObj.Metadata.AssemblyFile;

                        if (File.Exists(assemblyFile)) {
                            var assembly = Assembly.LoadFrom(assemblyFile);

                            var desiredTypeName = baseObj.Metadata.Type;
                            var qualifiedName = Assembly.CreateQualifiedName(assembly.FullName, desiredTypeName);
                            var desiredType = Type.GetType(qualifiedName, false, true);

                            if (desiredType != null) {
                                if (!dict.ContainsKey(desiredType)) {
                                    reader.BaseStream.Position = 0;

                                    var obj = deserializer.Deserialize(reader, desiredType);
                                    var obj2 = (ConfigBase)obj;
                                    dict[desiredType] = obj2;
                                } else {
                                    GameLog.Warn("{0}: Config file '{1}': Config object '{2}' already exists.", ReflectionHelper.GetCallerName(), p, desiredTypeName);
                                }
                            } else {
                                GameLog.Warn("{0}: Config file '{1}': Desired type '{2}' does not exist.", ReflectionHelper.GetCallerName(), p, desiredTypeName);
                            }
                        } else {
                            GameLog.Warn("{0}: Config file '{1}': Cannot find '{2}'.", ReflectionHelper.GetCallerName(), p, assemblyFile);
                        }
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

        public bool TryGetValue<T>(out T value) where T : ConfigBase {
            var b = TryGetValue(typeof(T), out var val);
            value = (T)val;

            return b;
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

                var globCharIndex = path.IndexOfAny(GlobHelper.PartialGlobChars);

                // The path has been expanded, e.g. xxx/**/*yyy.zzz -> C:\dir\xxx\**\*.yyy.zzz,
                // so a glob char should not appear be the first char.
                if (globCharIndex == 0) {
                    throw new ApplicationException("Unexpected: glob char index == 0");
                }

                if (globCharIndex > 0) {
                    var sepIndex = path.LastIndexOfAny(GlobHelper.PathSeparatorChars, globCharIndex - 1);
                    if (sepIndex < 0) {
                        throw new ArgumentOutOfRangeException(nameof(sepIndex));
                    }

                    basePath = path.Substring(0, sepIndex + 1);

                    var dirInfo = new DirectoryInfo(basePath);

                    if (dirInfo.Exists) {
                        var subPath = path.Substring(sepIndex + 1);

                        foreach (var fileInfo in dirInfo.GlobFiles(subPath)) {
                            if (result.Contains(fileInfo.FullName)) {
                                continue;
                            }

                            q.Enqueue((Path: fileInfo.Name, BasePath: fileInfo.DirectoryName));
                        }
                    } else {
                        GameLog.Warn("{0}: Config file {1}: Cannot find directory '{2}'.", ReflectionHelper.GetCallerName(), entryFilePath, dirInfo.FullName);
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

        private static readonly Regex IncludeRegex = new Regex(@"^@include\s+\<(?<path>[^>]*)\>$");

    }
}

