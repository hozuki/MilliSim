using System;
using System.Drawing;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Theater.Elements.Logical;
using OpenMLTD.MilliSim.Theater.Extensions;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace OpenMLTD.MilliSim.Theater.Elements.Visual.Overlays {
    public class SongTitle : OutlinedTextOverlay, IBufferedElement2D {

        public SongTitle(GameBase game)
            : base(game) {
        }

        public float Opacity {
            get => _opacity;
            set => _opacity = value.Clamp(0, 1);
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);

            var bitmapProps = new BitmapProperties1();
            bitmapProps.PixelFormat = new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied);
            bitmapProps.BitmapOptions = BitmapOptions.Target;
            _offscreenBitmap = new Bitmap1(context.RenderTarget.DeviceContext2D, context.ClientSize.ToD2DSize(), bitmapProps);

            var settings = Program.Settings;
            var clientSize = context.ClientSize;

            var layout = settings.UI.SongTitle.Layout;
            var x = layout.X.IsPercentage ? clientSize.Width * layout.X.Value : layout.X.Value;
            var y = layout.Y.IsPercentage ? clientSize.Height * layout.Y.Value : layout.Y.Value;

            Location = new Point((int)x, (int)y);
        }

        protected override void OnLostContext(RenderContext context) {
            _offscreenBitmap.Dispose();
            base.OnLostContext(context);
        }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            var syncTimer = Game.AsTheaterDays().FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                throw new InvalidOperationException();
            }

            var now = syncTimer.CurrentTime.TotalSeconds;

            var settings = Program.Settings;
            var stages = settings.Animation.SongTitle;
            var reappearStages = settings.Animation.SongTitleReappear;

            var s1t1 = stages.Enter + stages.FadeIn;
            var s1t2 = s1t1 + stages.Hold;
            var s1t3 = s1t2 + stages.FadeOut;
            var s2t1 = reappearStages.Enter + reappearStages.FadeIn;
            var s2t2 = s2t1 + reappearStages.Hold;
            var s2t3 = s2t2 + reappearStages.FadeOut;

            if (!(stages.Enter <= now && now <= s1t3) && !(reappearStages.Enter <= now && now <= s2t3)) {
                Opacity = 0;
                return;
            }

            float opacity = 0;
            if (!stages.FadeIn.Equals(0) && now <= s1t1) {
                opacity = (float)(now - stages.Enter) / (float)stages.FadeIn;
            } else if (!stages.Hold.Equals(0) && now <= s1t2) {
                opacity = 1;
            } else if (!stages.FadeOut.Equals(0) && now <= s1t3) {
                opacity = 1 - (float)(now - s1t2) / (float)stages.FadeOut;
            } else if (!reappearStages.FadeIn.Equals(0) && now <= s2t1) {
                opacity = (float)(now - reappearStages.Enter) / (float)reappearStages.FadeIn;
            } else if (!reappearStages.Hold.Equals(0) && now <= s2t2) {
                opacity = 1;
            } else if (!reappearStages.FadeOut.Equals(0) && now <= s2t3) {
                opacity = 1 - (float)(now - s2t2) / (float)reappearStages.FadeOut;
            }

            Opacity = opacity;
        }

        protected sealed override void OnDraw(GameTime gameTime, RenderContext context) {
            if (_opacity <= 0) {
                return;
            }

            using (TargetSwitcher.Begin2D(context, _offscreenBitmap)) {
                context.Begin2D();
                context.Clear2D(Color.Transparent);
                context.End2D();
                base.OnDraw(gameTime, context);
            }
            OnCopyBufferedContents(context, _offscreenBitmap);
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            var settings = Program.Settings;
            Text = settings.Game.Title;
            FontSize = settings.UI.SongTitle.FontSize;
            StrokeWidth = settings.UI.SongTitle.StrokeWidth;
        }

        private void OnCopyBufferedContents(RenderContext context, SharpDX.Direct2D1.Bitmap buffer) {
            context.Begin2D();
            context.DrawBitmap(buffer, Opacity);
            context.End2D();
        }

        void IBufferedElement2D.OnCopyBufferedContents(GameTime gameTime, RenderContext context, Bitmap buffer) {
            OnCopyBufferedContents(context, buffer);
        }

        private Bitmap1 _offscreenBitmap;
        private float _opacity;

    }
}
