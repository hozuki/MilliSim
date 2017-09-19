using System;
using System.Drawing;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D.Effects;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Theater.Extensions;
using SharpDX.Direct2D1;
using RectangleF = System.Drawing.RectangleF;

namespace OpenMLTD.MilliSim.Theater.Elements.Visual.Gaming {
    public class TapPoints : BufferedElement2D {

        public TapPoints(GameBase game)
            : base(game) {
        }

        public int TrackCount {
            get => _trackCount;
            set {
                if (value < 1) {
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
                _trackCount = value;
            }
        }

        public float[] EndXRatios => _tapPointsX;

        public float[] StartXRatios => _incomingX;

        protected override void OnLayout() {
            base.OnLayout();

            var settings = Program.Settings;

            var workingAreaWidth = settings.UI.TapPoints.Layout.Width.Value;
            var l = (1 - workingAreaWidth) / 2;
            var r = l + workingAreaWidth;
            var n = TrackCount;

            var tracks = new float[n];
            for (var i = 0; i < n; ++i) {
                tracks[i] = l + (r - l) * i / (n - 1);
            }

            var midPoints = new float[tracks.Length - 1];
            for (var i = 0; i < midPoints.Length; ++i) {
                midPoints[i] = (tracks[i] + tracks[i + 1]) / 2;
            }

            _tapPointsX = tracks;
            _tapNodesX = midPoints;

            var incomings = new float[tracks.Length];
            for (var i = 0; i < incomings.Length; ++i) {
                incomings[i] = 0.5f + (tracks[i] - 0.5f) * 0.6f;
            }
            _incomingX = incomings;
        }

        protected override void OnDrawBuffer(GameTime gameTime, RenderContext context) {
            base.OnDrawBuffer(gameTime, context);

            var settings = Program.Settings;
            var clientSize = context.ClientSize;

            var centerY = settings.UI.TapPoints.Layout.Y * clientSize.Height;

            float[] tapPointXPercArray = _tapPointsX, nodeXPercArray = _tapNodesX;

            var gamingArea = Game.AsTheaterDays().FindSingleElement<GamingArea>();

            if (gamingArea == null) {
                throw new InvalidOperationException();
            }

            var scaleResults = gamingArea.ScaleResults;

            // Draw "chains", left and right.
            context.PushTransform2D();
            context.Translate2D(0, centerY - scaleResults.TapBarChain.Height / 2);
            context.Begin2D();
            var yy = scaleResults.TapBarChain.Height / 2;
            var blankLeft = settings.Images.TapPoint.BlankEdge.Left;
            var blankRight = settings.Images.TapPoint.BlankEdge.Right;
            for (var i = 0; i < nodeXPercArray.Length; ++i) {
                var x1 = clientSize.Width * tapPointXPercArray[i];
                var x2 = clientSize.Width * nodeXPercArray[i];
                x1 += scaleResults.TapPoint.Start.Width / 2 - blankLeft;
                x2 -= scaleResults.TapBarNode.Width / 2;
                // Already translated.
                context.DrawLine(_tapBarChainPen, x1, yy, x2, yy);

                x1 = clientSize.Width * nodeXPercArray[i];
                x2 = clientSize.Width * tapPointXPercArray[i + 1];
                x1 += scaleResults.TapBarNode.Width / 2;
                x2 -= scaleResults.TapPoint.Start.Width / 2 - blankRight;
                // Already translated.
                context.DrawLine(_tapBarChainPen, x1, yy, x2, yy);
            }
            context.End2D();
            context.PopTransform2D();

            // Draw nodes.
            context.Begin2D();
            for (var i = 0; i < nodeXPercArray.Length; ++i) {
                var centerX = clientSize.Width * nodeXPercArray[i];
                context.DrawBitmap(_tapBarNodeImage, centerX - scaleResults.TapBarNode.Width / 2, centerY - scaleResults.TapBarNode.Height / 2, scaleResults.TapBarNode.Width, scaleResults.TapBarNode.Height);
            }
            context.End2D();

            // Draw tap areas.
            context.Begin2D();
            for (var i = 0; i < tapPointXPercArray.Length; ++i) {
                var centerX = clientSize.Width * tapPointXPercArray[i];
                context.DrawBitmap(_tapPointImage, centerX - scaleResults.TapPoint.Start.Width / 2, centerY - scaleResults.TapPoint.Start.Height / 2, scaleResults.TapPoint.Start.Width, scaleResults.TapPoint.Start.Height);
            }
            context.End2D();
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);

            var settings = Program.Settings;
            var gamingArea = Game.AsTheaterDays().FindSingleElement<GamingArea>();

            if (gamingArea == null) {
                throw new InvalidOperationException();
            }

            Opacity = settings.UI.TapPoints.Opacity;

            _tapPointImage = Direct2DHelper.LoadBitmap(context, settings.Images.TapPoint.FileName);

            var tapBarChainImage = Direct2DHelper.LoadBitmap(context, settings.Images.TapBarChain.FileName);
            _tapBarChainImage = tapBarChainImage;

            _tapBarNodeImage = Direct2DHelper.LoadBitmap(context, settings.Images.TapBarNode.FileName);

            var scaledTapBarChainSize = gamingArea.ScaleResults.TapBarChain;
            var tapBarChainScaledRatio = new SizeF(scaledTapBarChainSize.Width / tapBarChainImage.Width, scaledTapBarChainSize.Height / tapBarChainImage.Height);
            _tapBarChainScaleEffect = new D2DScaleEffect(context) {
                Scale = tapBarChainScaledRatio
            };
            _tapBarChainScaleEffect.SetInput(0, tapBarChainImage);

            var tapBarBrushRect = new RectangleF(0, 0, scaledTapBarChainSize.Width, scaledTapBarChainSize.Height);
            _tapBarChainBrush = new D2DImageBrush(context, _tapBarChainScaleEffect, ExtendMode.Wrap, ExtendMode.Wrap, InterpolationMode.Linear, tapBarBrushRect);
            _tapBarChainPen = new D2DPen(_tapBarChainBrush, scaledTapBarChainSize.Height);
        }

        protected override void OnLostContext(RenderContext context) {
            base.OnLostContext(context);
            _tapBarChainPen.Dispose();
            _tapBarChainBrush.Dispose();
            _tapBarChainScaleEffect.Dispose();
            _tapPointImage?.Dispose();
            _tapBarChainImage?.Dispose();
            _tapBarNodeImage?.Dispose();
        }

        private float[] _tapPointsX;
        private float[] _incomingX;

        private float[] _tapNodesX;

        [CanBeNull]
        private D2DBitmap _tapPointImage;
        [CanBeNull]
        private D2DBitmap _tapBarChainImage;
        [CanBeNull]
        private D2DBitmap _tapBarNodeImage;

        private D2DScaleEffect _tapBarChainScaleEffect;

        private D2DImageBrush _tapBarChainBrush;
        private D2DPen _tapBarChainPen;

        private int _trackCount = 2;

    }
}