using System;
using System.Drawing;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics.Extensions;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace OpenMLTD.MilliSim.Graphics.Drawing {
    /// <summary>
    /// A buffered 2D element. This class must be inherited.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Buffered elements can have more kinds of effects, such as opacity, color correction and styling.
    /// Each <see cref="BufferedElement2D"/> has an underlying <see cref="SharpDX.Direct2D1.Bitmap"/> as the buffer. So when
    /// using <see cref="BufferedElement2D"/> class, you can only use 2D drawing functions in <see cref="RenderContext"/>, and
    /// this is why there is a "2D" in class name.
    /// </para>
    /// <para>
    /// However, please note that in current implementation, every buffer is as large as the client area. So heavy use of this
    /// class will cause severe efficiency drop. Maybe there will be bounding box calculation and some other optimization
    /// stuff in the future.
    /// </para>
    /// </remarks>
    public abstract class BufferedElement2D : VisualElement, IBufferedElement2D, IElement2D {

        protected BufferedElement2D(GameBase game)
            : base(game) {
        }

        public virtual Point Location { get; set; }

        public float Opacity {
            get => _opacity;
            set => _opacity = value.Clamp(0, 1);
        }

        public void FadeTo(float finalOpacity, float deltaPerSecond) {
            var opacity = Opacity;
            finalOpacity = finalOpacity.Clamp(0, 1);

            if ((finalOpacity - opacity) * deltaPerSecond <= 0) {
                return;
            }

            _finalOpacity = finalOpacity;
            _deltaPerSecond = deltaPerSecond;
            _isFading = true;
        }

        public void FadeTo(float finalOpacity, TimeSpan duration) {
            var opacity = Opacity;
            finalOpacity = finalOpacity.Clamp(0, 1);

            if (opacity.Equals(finalOpacity)) {
                return;
            }

            var delta = finalOpacity - opacity;
            var deltaPerSecond = delta / (float)duration.TotalSeconds;
            FadeTo(finalOpacity, deltaPerSecond);
        }

        public void FadeOut(float deltaPerSecond) {
            FadeTo(0, deltaPerSecond);
        }

        public void FadeOut(TimeSpan duration) {
            FadeTo(0, duration);
        }

        public void FadeIn(float deltaPerSecond) {
            FadeTo(1, deltaPerSecond);
        }

        public void FadeIn(TimeSpan duration) {
            FadeTo(1, duration);
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);
            var bitmapProps = new BitmapProperties1();
            bitmapProps.PixelFormat = new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied);
            bitmapProps.BitmapOptions = BitmapOptions.Target;
            _offscreenBitmap = new Bitmap1(context.RenderTarget.DeviceContext2D, context.ClientSize.ToD2DSize(), bitmapProps);
        }

        protected override void OnLostContext(RenderContext context) {
            _offscreenBitmap.Dispose();
            base.OnLostContext(context);
        }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            if (_isFading) {
                var nextOpacity = Opacity + _deltaPerSecond * (float)gameTime.Delta.TotalSeconds;
                if (_deltaPerSecond > 0 && nextOpacity >= _finalOpacity) {
                    if (nextOpacity > _finalOpacity) {
                        nextOpacity = _finalOpacity;
                    }
                    _isFading = false;
                } else if (_deltaPerSecond < 0 && nextOpacity <= _finalOpacity) {
                    if (nextOpacity < _finalOpacity) {
                        nextOpacity = _finalOpacity;
                    }
                    _isFading = false;
                }
                Opacity = nextOpacity;
            }
        }

        protected sealed override void OnDraw(GameTime gameTime, RenderContext context) {
            using (TargetSwitcher.Begin2D(context, _offscreenBitmap)) {
                context.Begin2D();
                context.Clear2D(Color.Transparent);
                context.End2D();
                base.OnDraw(gameTime, context);
                OnDrawBuffer(gameTime, context);
            }
            OnCopyBufferedContents(gameTime, context, _offscreenBitmap);
        }

        protected virtual void OnDrawBuffer(GameTime gameTime, RenderContext context) {
        }

        protected virtual void OnCopyBufferedContents(GameTime gameTime, RenderContext context, Bitmap buffer) {
            context.Begin2D();
            context.DrawBitmap(buffer, Opacity);
            context.End2D();
        }

        void IBufferedElement2D.OnCopyBufferedContents(GameTime gameTime, RenderContext context, Bitmap buffer) {
            OnCopyBufferedContents(gameTime, context, buffer);
        }

        private Bitmap1 _offscreenBitmap;
        private float _opacity = 1;

        private bool _isFading;
        private float _deltaPerSecond;
        private float _finalOpacity;

    }
}
