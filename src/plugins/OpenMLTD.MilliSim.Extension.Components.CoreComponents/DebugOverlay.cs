using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="TextOverlay" /> that displays debug information.
    /// </summary>
    public class DebugOverlay : TextOverlay {

        /// <inheritdoc />
        /// <summary>
        /// Creates a new <see cref="DebugOverlay"/> instance.
        /// </summary>
        /// <param name="game">The base game.</param>
        /// <param name="parent">The parent of the <see cref="DebugOverlay"/>.</param>
        public DebugOverlay([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        /// <summary>
        /// Gets or sets the maximum number of lines displaying in the <see cref="DebugOverlay"/>.
        /// </summary>
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

        /// <summary>
        /// Gets current line count.
        /// </summary>
        public int LineCount => _lines.Count;

        /// <summary>
        /// Adds a line of text to the bottom.
        /// </summary>
        /// <param name="text">The text to add.</param>
        public void AddLine([CanBeNull] string text) {
            if (text == null) {
                return;
            }

            while (_lines.Count >= MaxLines) {
                _lines.RemoveAt(0);
            }

            _lines.Add(text);
            _linesChanged = true;
        }

        /// <summary>
        /// Removes the first line.
        /// </summary>
        public void RemoveFirstLine() {
            if (_lines.Count == 0) {
                return;
            }

            _lines.RemoveAt(0);
            _linesChanged = true;
        }

        /// <summary>
        /// Removes the last line.
        /// </summary>
        public void RemoveLastLine() {
            if (_lines.Count == 0) {
                return;
            }

            _lines.RemoveAt(_lines.Count - 1);
            _linesChanged = true;
        }

        /// <summary>
        /// Removes the line at the specified index.
        /// </summary>
        /// <param name="index">Line index.</param>
        public void RemoveLineAt(int index) {
            if (index < 0 || index >= _lines.Count) {
                return;
            }

            _lines.RemoveAt(index);
            _linesChanged = true;
        }

        /// <summary>
        /// Gets the text of the line at the specified index.
        /// </summary>
        /// <param name="index">Line index.</param>
        /// <returns>The text of that line.</returns>
        [NotNull]
        public string GetLine(int index) {
            if (index < 0 || index >= _lines.Count) {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }

            return _lines[index];
        }

        /// <summary>
        /// Sets the text of a specific line.
        /// </summary>
        /// <param name="index">Line index.</param>
        /// <param name="text">The new text.</param>
        public void SetLine(int index, [CanBeNull] string text) {
            if (text == null) {
                return;
            }

            if (index < 0 || index >= _lines.Count) {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }

            var originalText = _lines[index];

            if (originalText != text) {
                _lines[index] = text;
                _linesChanged = true;
            }
        }

        /// <summary>
        /// Clears all the lines.
        /// </summary>
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
