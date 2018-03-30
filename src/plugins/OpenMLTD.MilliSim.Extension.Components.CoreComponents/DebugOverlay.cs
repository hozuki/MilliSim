using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public class DebugOverlay : TextOverlay {

        public DebugOverlay([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
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
            _linesChanged = true;
        }

        public void RemoveFirstLine() {
            if (_lines.Count == 0) {
                return;
            }
            _lines.RemoveAt(0);
            _linesChanged = true;
        }

        public void RemoveLastLine() {
            if (_lines.Count == 0) {
                return;
            }
            _lines.RemoveAt(_lines.Count - 1);
            _linesChanged = true;
        }

        public void RemoveLine(int index) {
            _lines.RemoveAt(index);
            _linesChanged = true;
        }

        public string GetLine(int index) {
            return _lines[index];
        }

        public void SetLine(int index, string text) {
            text = text ?? string.Empty;
            _lines[index] = text;
            _linesChanged = true;
        }

        public void Clear() {
            _lines.Clear();
            _linesChanged = true;
        }

        protected override void OnUpdate(GameTime gameTime) {
            if (_linesChanged) {
                Text = string.Join(Environment.NewLine, _lines.ToArray());
                _linesChanged = false;
            }

            base.OnUpdate(gameTime);
        }

        private bool _linesChanged;
        private readonly List<string> _lines = new List<string>();
        private int _maxLines = 15;

    }
}
