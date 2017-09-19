using System;
using System.IO;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities.Extending;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Source;
using OpenMLTD.MilliSim.Extension.Imports.Unity3D.Extensions;
using OpenMLTD.MilliSim.Extension.Imports.Unity3D.Serialization;
using UnityStudio.Extensions;
using UnityStudio.Models;
using UnityStudio.Serialization;

namespace OpenMLTD.MilliSim.Extension.Imports.Unity3D {
    public sealed class Unity3DScoreReader : DisposableBase, IScoreReader {

        internal Unity3DScoreReader() {
        }

        public SourceScore ReadSourceScore(Stream stream, string fileName, ReadSourceOptions options) {
            var extension = Path.GetExtension(fileName);
            if (extension == null) {
                throw new ArgumentException();
            }

            var isCompressed = extension.ToLowerInvariant().EndsWith(".lz4");
            ScoreObject scoreObject = null;

            using (var bundle = new BundleFile(stream, fileName, isCompressed)) {
                foreach (var asset in bundle.AssetFiles) {
                    foreach (var preloadData in asset.PreloadDataList) {
                        if (preloadData.KnownType == KnownClassID.MonoBehaviour) {
                            var behavior = preloadData.LoadAsMonoBehavior(true);
                            if (behavior.Name.Contains("fumen")) {
                                behavior = preloadData.LoadAsMonoBehavior(false);
                                var serializer = new MonoBehaviorSerializer();
                                scoreObject = serializer.Deserialize<ScoreObject>(behavior);
                                break;
                            }
                        }
                    }
                }
            }

            if (scoreObject == null) {
                throw new FormatException();
            }

            return scoreObject.ToSourceScore(options);
        }

        public RuntimeScore ReadCompiledScore(Stream stream, string fileName, ReadSourceOptions sourceOptions, ScoreCompileOptions compileOptions) {
            var score = ReadSourceScore(stream, fileName, sourceOptions);
            using (var compiler = new Unity3DScoreCompiler()) {
                return compiler.Compile(score, compileOptions);
            }
        }

        protected override void Dispose(bool disposing) {
        }

    }
}
