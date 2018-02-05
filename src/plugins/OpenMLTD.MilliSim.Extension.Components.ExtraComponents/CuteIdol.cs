using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Extension.Components.ExtraComponents.Properties;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Advanced;
using OpenMLTD.MilliSim.Graphics.Extensions;

namespace OpenMLTD.MilliSim.Extension.Components.ExtraComponents {
    public sealed class CuteIdol : Visual {

        public CuteIdol([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
            Visible = false;
        }

        public int PickRandomCharacter() {
            // 85% chance to roll out Project 39 members (39)
            // 12% chance to roll out 765 All Stars members (13)
            // 3% chance to roll out the easter egg of the easter egg (1)
            var firstRoll = MathHelperEx.Random.NextDouble();
            int upper, lower;

            if (firstRoll < 0.85) {
                lower = 0;
                upper = 39;
            } else if (firstRoll < 0.97) {
                lower = 39;
                upper = 52;
            } else {
                lower = 52;
                upper = 53;
            }

            var characterIndex = MathHelperEx.Random.Next(lower, upper);
            _selectedCharacterIndex = characterIndex;

            return characterIndex;
        }

        protected override void OnDraw(GameTime gameTime) {
            base.OnDraw(gameTime);

            var index = _selectedCharacterIndex;
            if (index < 0) {
                return;
            }

            if (_characterImages == null) {
                return;
            }

            if (index < 0 || index >= TotalCharacterCount) {
                return;
            }

            var game = Game.ToBaseGame();

            var viewport = game.GraphicsDevice.Viewport;
            float x = viewport.Width - CharacterWidth;
            float y = viewport.Height - CharacterHeight;

            var yDelta = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 5) / 5 * CharacterWidth;

            if (yDelta > 0) {
                yDelta = -yDelta;
            }

            y += yDelta;

            game.SpriteBatch.BeginOnBufferedVisual();
            game.SpriteBatch.Draw(_characterImages, index, x, y);
            game.SpriteBatch.End();
        }

        protected override void OnLoadContents() {
            try {
                _characterImages = ContentHelper.LoadSpriteSheet2D(Game.GraphicsDevice, Resources.CharacterIcons, Resources.CharacterIcons.RawFormat, CharacterWidth, CharacterHeight, Stride, TotalCharacterCount, CharacterImagesOrientation);
            } catch (Exception ex) {
                var game = Game as BaseGame;
                var debugOverlay = game?.FindSingleElement<DebugOverlay>();
                if (debugOverlay != null) {
                    debugOverlay.AddLine(ex.StackTrace);
                    debugOverlay.AddLine($"Failed to load easter egg: {ex.Message}");
                }
            }
        }

        protected override void OnUnloadContents() {
            _characterImages?.Dispose();
        }

        [CanBeNull]
        private SpriteSheet2D _characterImages;

        private int _selectedCharacterIndex = -1;

        private static readonly int BaseCharacterCount = 52;
        private static readonly int ExtraCharacterCount = 1;
        private static readonly int TotalCharacterCount = BaseCharacterCount + ExtraCharacterCount;
        private static readonly int CharacterWidth = 162;
        private static readonly int CharacterHeight = 162;
        // 12 columns
        private static readonly int Stride = 12;
        private static readonly SpriteSheetOrientation CharacterImagesOrientation = SpriteSheetOrientation.Horizontal;

    }
}
