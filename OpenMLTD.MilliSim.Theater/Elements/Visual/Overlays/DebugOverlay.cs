using System.Collections.Generic;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Theater.Elements.Visual.Overlays {
    public class DebugOverlay : TextOverlay {

        public DebugOverlay(GameBase game)
            : base(game) {
        }

        public int MaxLines {
            get => _maxLines;
            set {
                if (value < 0) {
                    value = 0;
                }
                _maxLines = value;
                while (_lines.Count > value) {
                    _lines.RemoveAt(0);
                }
            }
        }

        public int LineCount => _lines.Count;

        public void AddLine(string text) {
            while (_lines.Count >= MaxLines) {
                _lines.RemoveAt(0);
            }
            _lines.Add(text);
            UpdateText();
        }

        public void RemoveFirstLine() {
            if (_lines.Count == 0) {
                return;
            }
            _lines.RemoveAt(0);
            UpdateText();
        }

        public void RemoveLastLine() {
            if (_lines.Count == 0) {
                return;
            }
            _lines.RemoveAt(_lines.Count - 1);
            UpdateText();
        }

        public void RemoveLine(int index) {
            _lines.RemoveAt(index);
            UpdateText();
        }

        public string GetLine(int index) {
            return _lines[index];
        }

        public void SetLine(int index, string text) {
            text = text ?? string.Empty;
            _lines[index] = text;
            UpdateText();
        }

        public void Clear() {
            _lines.Clear();
            UpdateText();
        }

        private void UpdateText() {
            Text = string.Join("\n", _lines.ToArray());
        }

        private readonly List<string> _lines = new List<string>();
        private int _maxLines = 15;

    }
}
