using System;
using System.IO;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Core.Entities.Extending;
using OpenMLTD.MilliSim.Extension.Imports.Unity3D.Extensions;
using UnityStudio.Extensions;
using UnityStudio.Models;
using UnityStudio.Serialization;

namespace OpenMLTD.MilliSim.Extension.Imports.Unity3D {
    public sealed class Unity3DScoreReader : DisposableBase, IScoreReader {

        internal Unity3DScoreReader() {
        }

        public Score Read(Stream stream, string fileName, IFlexibleOptions options) {
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

            return scoreObject.ToMilliSimScore();
        }

        public bool TryRead(Stream stream, string fileName, IFlexibleOptions options, out Score score) {
            try {
                score = Read(stream, fileName, options);
                return true;
            } catch {
                score = null;
                return false;
            }
        }

        protected override void Dispose(bool disposing) {
        }

    }
}
