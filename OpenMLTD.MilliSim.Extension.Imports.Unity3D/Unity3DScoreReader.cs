using System;
using System.Composition;
using System.IO;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Core.Entities.Extending;
using OpenMLTD.MilliSim.Extension.Imports.Unity3D.Extensions;
using UnityStudio.Extensions;
using UnityStudio.Models;
using UnityStudio.Serialization;

namespace OpenMLTD.MilliSim.Extension.Imports.Unity3D {
    [Export(typeof(IScoreReader))]
    public sealed class Unity3DScoreReader : IScoreReader {

        public Score Read(Stream stream, string fileName) {
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

        public bool TryRead(Stream stream, string fileName, out Score score) {
            try {
                score = Read(stream, fileName);
                return true;
            } catch {
                score = null;
                return false;
            }
        }

        public bool SupportsFileType(string fileName) {
            fileName = fileName.ToLowerInvariant();
            return fileName.EndsWith(".unity3d") || fileName.EndsWith(".unity3d.lz4");
        }

        public string Description => "Unity3D Score Reader";

    }
}
