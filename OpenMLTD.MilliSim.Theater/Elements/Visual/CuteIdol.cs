using System;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D.Advanced;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Theater.Elements.Visual.Overlays;
using OpenMLTD.MilliSim.Theater.Extensions;
using OpenMLTD.MilliSim.Theater.Properties;

namespace OpenMLTD.MilliSim.Theater.Elements.Visual {
    internal class CuteIdol : Graphics.Visual {

        public CuteIdol([NotNull] IVisualContainer parent)
            : base(parent) {
        }

        public int PickRandomCharacter() {
            // 85% chance to roll out Project 39 members (39)
            // 12% chance to roll out 765 All Stars members (13)
            // 3% chance to roll out the easter egg of the easter egg (1)
            var firstRoll = MathHelper.Random.NextDouble();
            int upper, lower;

            if (firstRoll < 0.85) {
                lower = 0;
                upper = 39;
            } else if (firstRoll < 0.97) {
                lower = 39;
                upper = 52;
            } else {
                lower = 52;
                upper = 52;
            }

            var characterIndex = MathHelper.Random.Next(lower, upper);
            _selectedCharacterIndex = characterIndex;

            return characterIndex;
        }

        public override bool Visible { get; set; } = false;

        protected override void OnDraw(GameTime gameTime, RenderContext context) {
            base.OnDraw(gameTime, context);

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

            var clientSize = context.ClientSize;
            var x = (float)clientSize.Width - CharacterWidth;
            var y = (float)clientSize.Height - CharacterHeight;

            var yDelta = (float)Math.Sin(gameTime.Total.TotalSeconds * 5) / 5 * CharacterWidth;
            if (yDelta > 0) {
                yDelta = -yDelta;
            }
            y += yDelta;

            context.Begin2D();
            context.DrawImageStripUnit(_characterImages, index, x, y);
            context.End2D();
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);

            try {
                _characterImages = Direct2DHelper.LoadImageStrip2D(context, Resources.CharacterIcons, CharacterWidth, CharacterHeight, TotalCharacterCount, ArrayCount, CharacterImagesOrientation);
            } catch (Exception ex) {
                var debugOverlay = Game.AsTheaterDays().FindSingleElement<DebugOverlay>();
                if (debugOverlay != null) {
                    debugOverlay.AddLine(ex.StackTrace);
                    debugOverlay.AddLine($"Failed to load easter egg: {ex.Message}");
                }
            }
        }

        protected override void OnLostContext(RenderContext context) {
            _characterImages?.Dispose();

            base.OnLostContext(context);
        }

        [CanBeNull]
        private D2DImageStrip2D _characterImages;

        private int _selectedCharacterIndex = -1;

        private static readonly int BaseCharacterCount = 52;
        private static readonly int ExtraCharacterCount = 1;
        private static readonly int TotalCharacterCount = BaseCharacterCount + ExtraCharacterCount;
        private static readonly int CharacterWidth = 162;
        private static readonly int CharacterHeight = 162;
        // 12 columns
        private static readonly int ArrayCount = 12;
        private static readonly ImageStripOrientation CharacterImagesOrientation = ImageStripOrientation.Horizontal;

    }
}
