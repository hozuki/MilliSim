using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Graphics {
    public sealed class CachedPrivateFontCollection : DisposableBase {

        public CachedPrivateFontCollection() {
            _collection = new PrivateFontCollection();
        }

        public FontFamily GetFontFamilyFromFile([NotNull] string fileName) {
            fileName = Path.GetFullPath(fileName);
            if (_loadedFamilies.ContainsKey(fileName)) {
                return _loadedFamilies[fileName];
            }

            _collection.AddFontFile(fileName);
            var families = _collection.Families;

            var addedFamily = LocateRecentlyAddedFamily(families);
            _loadedFamilyNames.Add(addedFamily.Name);
            _loadedFamilies[fileName] = addedFamily;

            return addedFamily;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                _collection?.Dispose();
                _collection = null;
            }
            _loadedFamilyNames.Clear();
            _loadedFamilies.Clear();
        }

        // TODO: this function will throw an exception when two same fonts (with different file names) are added.
        private FontFamily LocateRecentlyAddedFamily([NotNull] FontFamily[] newFamilies) {
            foreach (var family in newFamilies) {
                if (!_loadedFamilyNames.Contains(family.Name)) {
                    return family;
                }
            }

            throw new InvalidOperationException("Cannot locate recently added font family.");
        }

        private readonly HashSet<string> _loadedFamilyNames = new HashSet<string>();
        private readonly Dictionary<string, FontFamily> _loadedFamilies = new Dictionary<string, FontFamily>();
        private PrivateFontCollection _collection;

    }
}
