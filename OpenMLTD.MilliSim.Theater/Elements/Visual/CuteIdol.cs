using System;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D.Advanced;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Theater.Properties;

namespace OpenMLTD.MilliSim.Theater.Elements.Visual {
    internal class CuteIdol : VisualElement {

        public CuteIdol(GameBase game)
            : base(game) {
        }

        public int SelectedCharacterIndex { get; set; }

        public int NumberOfCharacters => CharacterCount;

        public override bool Visible { get; set; } = false;

        protected override void OnDraw(GameTime gameTime, RenderContext context) {
            base.OnDraw(gameTime, context);

            var index = SelectedCharacterIndex;
            if (index < 0 || index >= CharacterCount) {
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

            _characterImages = Direct2DHelper.LoadImageStrip(context, Resources.CharacterIcons, CharacterCount, CharacterImagesOrientation);
        }

        protected override void OnLostContext(RenderContext context) {
            _characterImages.Dispose();

            base.OnLostContext(context);
        }

        private D2DImageStrip _characterImages;

        private static readonly int CharacterCount = 52;
        private static readonly int CharacterWidth = 162;
        private static readonly int CharacterHeight = 162;
        private static readonly ImageStripOrientation CharacterImagesOrientation = ImageStripOrientation.Vertical;

    }
}
