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

            var centerY = settings.UI.TapPoints.Layout.Y * clientSize.Height;

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
            context.Translate(0, centerY - _tapBarChainRealSize.Height / 2);
            context.Begin2D();
            var yy = _tapBarChainRealSize.Height / 2;
            var blankLeft = settings.Images.TapPoint.BlankEdge.Left;
            var blankRight = settings.Images.TapPoint.BlankEdge.Right;
            for (var i = 0; i < nodeXPercArray.Length; ++i) {
                var x1 = clientSize.Width * tapPointXPercArray[i];
                var x2 = clientSize.Width * nodeXPercArray[i];
                x1 += _tapPointRealSize.Width / 2 - blankLeft;
                x2 -= _tapBarNodeRealSize.Width / 2;
                // Already translated.
                context.DrawLine(_tapBarChainPen, x1, yy, x2, yy);

                x1 = clientSize.Width * nodeXPercArray[i];
                x2 = clientSize.Width * tapPointXPercArray[i + 1];
                x1 += _tapBarNodeRealSize.Width / 2;
                x2 -= _tapPointRealSize.Width / 2 - blankRight;
                // Already translated.
                context.DrawLine(_tapBarChainPen, x1, yy, x2, yy);
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
            var tapBarScalingBasis = settings.Scaling.TapBarChain;
            var tapBarNodeScalingBasis = settings.Scaling.TapBarNode;

            Opacity = settings.UI.TapPoints.Opacity;

            _tapPointImage = Direct2DHelper.LoadBitmap(settings.Images.TapPoint.FileName, context);
            _tapPointScaleRatio = new Vector2(
                (windowScalingBasis.Width / clientSize.Width) * (tapPointScalingBasis.Width / _tapPointImage.Width),
                (windowScalingBasis.Height / clientSize.Height) * (tapPointScalingBasis.Height / _tapPointImage.Height));
            _tapPointRealSize = new SizeF(_tapPointImage.Width * _tapPointScaleRatio.X, _tapPointImage.Height * _tapPointScaleRatio.Y);

            _tapBarChainImage = Direct2DHelper.LoadBitmap(settings.Images.TapBarChain.FileName, context);
            _tapBarChainScaleRatio = new Vector2(
                (windowScalingBasis.Width / clientSize.Width) * (tapBarScalingBasis.Width / _tapBarChainImage.Width),
                (windowScalingBasis.Height / clientSize.Height) * (tapBarScalingBasis.Height / _tapBarChainImage.Height));
            _tapBarChainRealSize = new SizeF(_tapBarChainImage.Width * _tapBarChainScaleRatio.X, _tapBarChainImage.Height * _tapBarChainScaleRatio.Y);

            _tapBarNodeImage = Direct2DHelper.LoadBitmap(settings.Images.TapBarNode.FileName, context);
            _tapBarNodeScaleRatio = new Vector2(
                (windowScalingBasis.Width / clientSize.Width) * (tapBarNodeScalingBasis.Width / _tapBarNodeImage.Width),
                (windowScalingBasis.Height / clientSize.Height) * (tapBarNodeScalingBasis.Height / _tapBarNodeImage.Height));
            _tapBarNodeRealSize = new SizeF(_tapBarNodeImage.Width * _tapBarNodeScaleRatio.X, _tapBarNodeImage.Height * _tapBarNodeScaleRatio.Y);

            _tapBarChainScaleEffect = new D2DScaleEffect(context) {
                Scale = ((RawVector2)_tapBarChainScaleRatio).ToGdiSizeF()
            };
            _tapBarChainScaleEffect.SetInput(0, _tapBarChainImage);

            var tapBarBrushRect = new RectangleF(0, 0, _tapBarChainRealSize.Width, _tapBarChainRealSize.Height);
            _tapBarChainBrush = new D2DImageBrush(context, _tapBarChainScaleEffect, ExtendMode.Wrap, ExtendMode.Wrap, InterpolationMode.Linear, tapBarBrushRect);
            _tapBarChainPen = new D2DPen(_tapBarChainBrush, _tapBarChainImage.Height * _tapBarChainScaleRatio.Y);
        }

        protected override void OnLostContext(RenderContext context) {
            base.OnLostContext(context);
            _tapBarChainPen.Dispose();
            _tapBarChainBrush.Dispose();
            _tapBarChainScaleEffect.Dispose();
            _tapPointImage.Dispose();
            _tapBarChainImage.Dispose();
            _tapBarNodeImage.Dispose();
        }

        private SizeF _tapPointRealSize;
        private SizeF _tapBarChainRealSize;
        private SizeF _tapBarNodeRealSize;

        private Vector2 _tapPointScaleRatio;
        private Vector2 _tapBarChainScaleRatio;
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
        private D2DBitmap _tapBarChainImage;
        private D2DBitmap _tapBarNodeImage;

        private D2DScaleEffect _tapBarChainScaleEffect;

        private D2DImageBrush _tapBarChainBrush;
        private D2DPen _tapBarChainPen;

    }
}
