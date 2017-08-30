using System;
using System.Drawing;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Rendering;
using OpenMLTD.MilliSim.Rendering.Drawing;
using OpenMLTD.MilliSim.Rendering.Drawing.Effects;
using OpenMLTD.MilliSim.Rendering.Extensions;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using RectangleF = System.Drawing.RectangleF;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public class TapPoints : BufferedElement2D {

        protected override void OnDrawBuffer(GameTime gameTime, RenderContext context) {
            var settings = Program.Settings;
            var clientSize = context.ClientSize;
            var difficulty = settings.Game.Difficulty;

            var centerY = settings.Layout.TapPoints.Y * clientSize.Height;

            float[] tapPointXPercArray, nodeXPercArray;
            switch (difficulty) {
                case Difficulty.D2Mix:
                case Difficulty.D2MixPlus:
                    tapPointXPercArray = TapPointsXPercPrecalculated[0];
                    nodeXPercArray = TapNodesXPercPrecalculated[0];
                    break;
                case Difficulty.D4Mix:
                    tapPointXPercArray = TapPointsXPercPrecalculated[1];
                    nodeXPercArray = TapNodesXPercPrecalculated[1];
                    break;
                case Difficulty.D6Mix:
                case Difficulty.MillionMix:
                    tapPointXPercArray = TapPointsXPercPrecalculated[2];
                    nodeXPercArray = TapNodesXPercPrecalculated[2];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Draw "chains", left and right.
            context.PushTransform();
            context.Translate(0, centerY - _tapBarRealSize.Height / 2);
            context.Begin2D();
            var yy = _tapBarRealSize.Height / 2;
            for (var i = 0; i < nodeXPercArray.Length; ++i) {
                var x1 = clientSize.Width * tapPointXPercArray[i];
                var x2 = clientSize.Width * nodeXPercArray[i];
                x1 += _tapPointRealSize.Width / 2;
                x2 -= _tapBarNodeRealSize.Width / 2;
                // Already translated.
                context.DrawLine(_tapBarConnectionPen, x1, yy, x2, yy);

                x1 = clientSize.Width * nodeXPercArray[i];
                x2 = clientSize.Width * tapPointXPercArray[i + 1];
                x1 += _tapBarNodeRealSize.Width / 2;
                x2 -= _tapPointRealSize.Width / 2;
                // Already translated.
                context.DrawLine(_tapBarConnectionPen, x1, yy, x2, yy);
            }
            context.End2D();
            context.PopTransform();

            // Draw nodes.
            context.Begin2D();
            for (var i = 0; i < nodeXPercArray.Length; ++i) {
                var centerX = clientSize.Width * nodeXPercArray[i];
                context.DrawBitmap(_tapBarNodeImage, centerX - _tapBarNodeRealSize.Width / 2, centerY - _tapBarNodeRealSize.Height / 2, _tapBarNodeRealSize.Width, _tapBarNodeRealSize.Height);
            }
            context.End2D();

            // Draw tap areas.
            context.Begin2D();
            for (var i = 0; i < tapPointXPercArray.Length; ++i) {
                var centerX = clientSize.Width * tapPointXPercArray[i];
                context.DrawBitmap(_tapPointImage, centerX - _tapPointRealSize.Width / 2, centerY - _tapPointRealSize.Height / 2, _tapPointRealSize.Width, _tapPointRealSize.Height);
            }
            context.End2D();
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);

            var settings = Program.Settings;
            var clientSize = context.ClientSize;
            var windowScalingBasis = settings.Scaling.Base;
            var tapPointScalingBasis = settings.Scaling.TapPoint;
            var tapBarScalingBasis = settings.Scaling.TapBar;
            var tapBarNodeScalingBasis = settings.Scaling.TapBarNode;

            _tapPointImage = Direct2DHelper.LoadBitmap(settings.Images.TapPoint, context);
            _tapPointScaleRatio = new Vector2(
                (windowScalingBasis.Width / clientSize.Width) * (tapPointScalingBasis.Width / _tapPointExpectedImageSize.Width),
                (windowScalingBasis.Height / clientSize.Height) * (tapPointScalingBasis.Height / _tapPointExpectedImageSize.Height));
            _tapPointRealSize = new SizeF(_tapPointImage.Width * _tapPointScaleRatio.X, _tapPointImage.Height * _tapPointScaleRatio.Y);

            _tapBarImage = Direct2DHelper.LoadBitmap(settings.Images.TapBar, context);
            _tapBarScaleRatio = new Vector2(
                (windowScalingBasis.Width / clientSize.Width) * (tapBarScalingBasis.Width / _tapBarExpectedImageSize.Width),
                (windowScalingBasis.Height / clientSize.Height) * (tapBarScalingBasis.Height / _tapBarExpectedImageSize.Height));
            _tapBarRealSize = new SizeF(_tapBarImage.Width * _tapBarScaleRatio.X, _tapBarImage.Height * _tapBarScaleRatio.Y);

            _tapBarNodeImage = Direct2DHelper.LoadBitmap(settings.Images.TapBarNode, context);
            _tapBarNodeScaleRatio = new Vector2(
                (windowScalingBasis.Width / clientSize.Width) * (tapBarNodeScalingBasis.Width / _tapBarNodeExpectedImageSize.Width),
                (windowScalingBasis.Height / clientSize.Height) * (tapBarNodeScalingBasis.Height / _tapBarNodeExpectedImageSize.Height));
            _tapBarNodeRealSize = new SizeF(_tapBarNodeImage.Width * _tapBarNodeScaleRatio.X, _tapBarNodeImage.Height * _tapBarNodeScaleRatio.Y);

            _tapBarScaleEffect = new D2DScaleEffect(context) {
                Scale = ((RawVector2)_tapBarScaleRatio).ToGdiSizeF()
            };
            _tapBarScaleEffect.SetInput(0, _tapBarImage);

            var tapBarBrushRect = new RectangleF(0, 0, _tapBarRealSize.Width, _tapBarRealSize.Height);
            _tapBarConnectionBrush = new D2DImageBrush(context, _tapBarScaleEffect, ExtendMode.Wrap, ExtendMode.Wrap, InterpolationMode.Linear, tapBarBrushRect);
            _tapBarConnectionPen = new D2DPen(_tapBarConnectionBrush, _tapBarExpectedImageSize.Height * _tapBarScaleRatio.Y);
        }

        protected override void OnLostContext(RenderContext context) {
            base.OnLostContext(context);
            _tapBarConnectionPen.Dispose();
            _tapBarConnectionBrush.Dispose();
            _tapBarScaleEffect.Dispose();
            _tapPointImage.Dispose();
            _tapBarImage.Dispose();
            _tapBarNodeImage.Dispose();
        }

        private readonly SizeF _tapPointExpectedImageSize = new SizeF(380, 380);
        private readonly SizeF _tapBarExpectedImageSize = new SizeF(32, 32);
        private readonly SizeF _tapBarNodeExpectedImageSize = new SizeF(74, 74);
        private SizeF _tapPointRealSize;
        private SizeF _tapBarRealSize;
        private SizeF _tapBarNodeRealSize;

        private Vector2 _tapPointScaleRatio;
        private Vector2 _tapBarScaleRatio;
        private Vector2 _tapBarNodeScaleRatio;

        private static readonly float[][] TapPointsXPercPrecalculated = {
            new[] {0.2f, 0.8f}, // 2mix, 2mix+
            new[] {0.2f, 0.4f, 0.6f, 0.8f}, //4mix,
            new[] {0.2f, 0.32f, 0.44f, 0.56f, 0.68f, 0.8f} // 6mix, mmix
        };

        private static readonly float[][] TapNodesXPercPrecalculated = {
            new[] {0.5f}, // 2mix, 2mix+
            new[] {0.3f, 0.5f, 0.7f}, //4mix,
            new[] {0.26f, 0.38f, 0.5f, 0.62f, 0.74f} // 6mix, mmix
        };

        private D2DBitmap _tapPointImage;
        private D2DBitmap _tapBarImage;
        private D2DBitmap _tapBarNodeImage;

        private D2DScaleEffect _tapBarScaleEffect;

        private D2DImageBrush _tapBarConnectionBrush;
        private D2DPen _tapBarConnectionPen;

    }
}
