using System;
using System.Diagnostics;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Theater.Elements.Visual.Gaming;
using OpenMLTD.MilliSim.Theater.Extensions;
using SharpDX;
using SharpDX.Direct2D1;
using Color = System.Drawing.Color;
using SweepDirection = SharpDX.Direct2D1.SweepDirection;

namespace OpenMLTD.MilliSim.Theater.Elements.Visual.Overlays {
    public class AvatarDisplay : BufferedElement2D {

        public AvatarDisplay(GameBase game)
            : base(game) {
        }

        protected override void OnDrawBuffer(GameTime gameTime, RenderContext context) {
            base.OnDrawBuffer(gameTime, context);

            var avatarRectangles = _avatarRectangles;

            context.Begin2D();

            var d2dContext = context.RenderTarget.DeviceContext2D;
            var d2dFactory = d2dContext.Factory;
            using (var geometry = new PathGeometry(d2dFactory)) {
                using (var sink = geometry.Open()) {
                    foreach (var rect in avatarRectangles) {
                        sink.BeginFigure(new Vector2(rect.X + rect.Width / 2, rect.Y), FigureBegin.Filled);
                        sink.AddArc(new ArcSegment {
                            ArcSize = ArcSize.Small,
                            Point = new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height),
                            Size = new Size2F(rect.Width / 2, rect.Height / 2),
                            SweepDirection = SweepDirection.CounterClockwise
                        });
                        sink.AddArc(new ArcSegment {
                            ArcSize = ArcSize.Small,
                            Point = new Vector2(rect.X + rect.Width / 2, rect.Y),
                            Size = new Size2F(rect.Width / 2, rect.Height / 2),
                            SweepDirection = SweepDirection.CounterClockwise
                        });
                        sink.EndFigure(FigureEnd.Closed);
                    }

                    sink.Close();
                }

                var originalAntiAliasMode = d2dContext.AntialiasMode;

                // https://msdn.microsoft.com/en-us/library/windows/desktop/hh847947.aspx
                var layerParams = new LayerParameters {
                    GeometricMask = geometry,
                    ContentBounds = RectangleF.Infinite,
                    MaskTransform = Matrix3x2.Identity,
                    MaskAntialiasMode = AntialiasMode.PerPrimitive,
                    Opacity = 1,
                    LayerOptions = LayerOptions.None
                };
                d2dContext.AntialiasMode = AntialiasMode.PerPrimitive;

                using (var layer = new Layer(d2dContext)) {
                    d2dContext.PushLayer(ref layerParams, layer);

                    var avatarImages = _avatarImages;

                    for (var i = 0; i < avatarImages.Length; ++i) {
                        var image = avatarImages[i];

                        if (image == null) {
                            continue;
                        }

                        var rect = avatarRectangles[i];

                        context.DrawBitmap(image, rect.X, rect.Y, rect.Width, rect.Height);
                    }

                    d2dContext.PopLayer();
                }

                d2dContext.AntialiasMode = originalAntiAliasMode;
            }

            context.End2D();

            context.Begin2D();
            foreach (var rect in avatarRectangles) {
                context.DrawEllipse(_borderPen, rect.X, rect.Y, rect.Width, rect.Height);
            }
            context.End2D();
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);

            var settings = Program.Settings;

            var avatarCount = settings.Images.Avatars.Length;
            var avatarImages = new D2DBitmap[avatarCount];

            for (var i = 0; i < avatarCount; ++i) {
                try {
                    var image = Direct2DHelper.LoadBitmap(context, settings.Images.Avatars[i].FileName);
                    avatarImages[i] = image;
                } catch (Exception ex) {
                    Debug.Print(ex.Message);
                }
            }

            var clientSize = context.ClientSize;
            var layout = settings.UI.Avatars.Layout;
            var x = layout.X.IsPercentage ? layout.X.Value * clientSize.Width : layout.X.Value;
            var y = layout.Y.IsPercentage ? layout.Y.Value * clientSize.Height : layout.Y.Value;
            var w = layout.Width.IsPercentage ? layout.Width.Value * clientSize.Width : layout.Width.Value;
            var h = layout.Height.IsPercentage ? layout.Height.Value * clientSize.Height : layout.Height.Value;

            var rects = new RectangleF[avatarCount];
            float radius;
            if (w >= h) {
                radius = h / 2;
                for (var i = 0; i < avatarCount; ++i) {
                    var cx = (w - radius * 2) / (avatarCount - 1) * i;
                    var rect = new RectangleF(x + cx, y, radius * 2, radius * 2);
                    rects[i] = rect;
                }
            } else {
                radius = w / 2;
                for (var i = 0; i < avatarCount; ++i) {
                    var cy = (h - radius * 2) / (avatarCount - 1) * i;
                    var rect = new RectangleF(x, y + cy, radius * 2, radius * 2);
                    rects[i] = rect;
                }
            }

            _avatarImages = avatarImages;
            _avatarRectangles = rects;

            var gamingArea = Game.AsTheaterDays().FindSingleElement<GamingArea>();
            if (gamingArea == null) {
                throw new InvalidOperationException();
            }

            var scalingResults = gamingArea.ScaleResults;

            _borderPen = new D2DPen(context, Color.White, scalingResults.AvatarBorder.Width);
        }

        protected override void OnLostContext(RenderContext context) {
            foreach (var image in _avatarImages) {
                image?.Dispose();
            }
            _avatarImages = null;

            _borderPen.Dispose();

            base.OnLostContext(context);
        }

        private D2DPen _borderPen;

        private RectangleF[] _avatarRectangles;

        [ItemCanBeNull]
        private D2DBitmap[] _avatarImages;

    }
}
