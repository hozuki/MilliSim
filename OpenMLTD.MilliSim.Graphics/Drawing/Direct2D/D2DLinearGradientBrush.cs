using System;
using System.Drawing;
using System.Linq;
using OpenMLTD.MilliSim.Graphics.Extensions;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace OpenMLTD.MilliSim.Graphics.Drawing.Direct2D {
    public sealed class D2DLinearGradientBrush : D2DBrushBase, ID2DBrush {

        public D2DLinearGradientBrush(RenderContext context, PointF startPoint, PointF endPoint, params Color[] gradientColors) {
            if (gradientColors.Length < 2) {
                throw new ArgumentException("Linear gradient brush requires at least 2 colors.", nameof(gradientColors));
            }
            var properties = new LinearGradientBrushProperties {
                StartPoint = new RawVector2(startPoint.X, startPoint.Y),
                EndPoint = new RawVector2(endPoint.X, endPoint.Y)
            };
            var colorCount = gradientColors.Length;
            var gradientStops = new GradientStop[colorCount];
            for (var i = 0; i < colorCount; ++i) {
                gradientStops[i] = new GradientStop {
                    Color = gradientColors[i].ToRC4(),
                    Position = (float)i / (colorCount - 1)
                };
            }
            var collection = new GradientStopCollection(context.RenderTarget.DeviceContext, gradientStops);
            NativeBrush = new LinearGradientBrush(context.RenderTarget.DeviceContext, properties, collection);
            _collection = collection;
        }

        public D2DLinearGradientBrush(RenderContext context, Point startPoint, Point endPoint, params Color[] gradientColors) {
            if (gradientColors.Length < 2) {
                throw new ArgumentException("Linear gradient brush requires at least 2 colors.", nameof(gradientColors));
            }
            var properties = new LinearGradientBrushProperties {
                StartPoint = new RawVector2(startPoint.X, startPoint.Y),
                EndPoint = new RawVector2(endPoint.X, endPoint.Y)
            };
            var colorCount = gradientColors.Length;
            var gradientStops = new GradientStop[colorCount];
            for (var i = 0; i < colorCount; ++i) {
                gradientStops[i] = new GradientStop {
                    Color = gradientColors[i].ToRC4(),
                    Position = (float)i / (colorCount - 1)
                };
            }
            var collection = new GradientStopCollection(context.RenderTarget.DeviceContext, gradientStops);
            NativeBrush = new LinearGradientBrush(context.RenderTarget.DeviceContext, properties, collection);
            _collection = collection;
        }

        public D2DLinearGradientBrush(RenderContext context, PointF startPoint, PointF endPoint, params (Color Color, float Position)[] gradientStops) {
            var properties = new LinearGradientBrushProperties {
                StartPoint = new RawVector2(startPoint.X, startPoint.Y),
                EndPoint = new RawVector2(endPoint.X, endPoint.Y)
            };
            var collection = new GradientStopCollection(context.RenderTarget.DeviceContext, gradientStops.Select(t => new GradientStop {
                Color = t.Color.ToRC4(),
                Position = t.Position
            }).ToArray());
            NativeBrush = new LinearGradientBrush(context.RenderTarget.DeviceContext, properties, collection);
            _collection = collection;
        }

        public D2DLinearGradientBrush(RenderContext context, Point startPoint, Point endPoint, params (Color Color, float Position)[] gradientStops) {
            var properties = new LinearGradientBrushProperties {
                StartPoint = new RawVector2(startPoint.X, startPoint.Y),
                EndPoint = new RawVector2(endPoint.X, endPoint.Y)
            };
            var collection = new GradientStopCollection(context.RenderTarget.DeviceContext, gradientStops.Select(t => new GradientStop {
                Color = t.Color.ToRC4(),
                Position = t.Position
            }).ToArray());
            NativeBrush = new LinearGradientBrush(context.RenderTarget.DeviceContext, properties, collection);
            _collection = collection;
        }

        public D2DLinearGradientBrush(RenderContext context, PointF startPoint, PointF endPoint, params GradientStop[] gradientStops) {
            var properties = new LinearGradientBrushProperties {
                StartPoint = new RawVector2(startPoint.X, startPoint.Y),
                EndPoint = new RawVector2(endPoint.X, endPoint.Y)
            };
            var collection = new GradientStopCollection(context.RenderTarget.DeviceContext, gradientStops);
            NativeBrush = new LinearGradientBrush(context.RenderTarget.DeviceContext, properties, collection);
            _collection = collection;
        }

        public D2DLinearGradientBrush(RenderContext context, Point startPoint, Point endPoint, params GradientStop[] gradientStops) {
            var properties = new LinearGradientBrushProperties {
                StartPoint = new RawVector2(startPoint.X, startPoint.Y),
                EndPoint = new RawVector2(endPoint.X, endPoint.Y)
            };
            var collection = new GradientStopCollection(context.RenderTarget.DeviceContext, gradientStops);
            NativeBrush = new LinearGradientBrush(context.RenderTarget.DeviceContext, properties, collection);
            _collection = collection;
        }

        public D2DLinearGradientBrush(RenderContext context, PointF startPoint, PointF endPoint, GradientStopCollection collection) {
            var properties = new LinearGradientBrushProperties {
                StartPoint = new RawVector2(startPoint.X, startPoint.Y),
                EndPoint = new RawVector2(endPoint.X, endPoint.Y)
            };
            NativeBrush = new LinearGradientBrush(context.RenderTarget.DeviceContext, properties, collection);
            _collection = collection;
        }

        public D2DLinearGradientBrush(RenderContext context, Point startPoint, Point endPoint, GradientStopCollection collection) {
            var properties = new LinearGradientBrushProperties {
                StartPoint = new RawVector2(startPoint.X, startPoint.Y),
                EndPoint = new RawVector2(endPoint.X, endPoint.Y)
            };
            NativeBrush = new LinearGradientBrush(context.RenderTarget.DeviceContext, properties, collection);
            _collection = collection;
        }

        public D2DLinearGradientBrush(RenderContext context, LinearGradientBrushProperties properties, GradientStopCollection collection) {
            NativeBrush = new LinearGradientBrush(context.RenderTarget.DeviceContext, properties, collection);
            _collection = collection;
        }

        internal LinearGradientBrush NativeBrush { get; }

        SharpDX.Direct2D1.Brush ID2DBrush.NativeBrush => NativeBrush;

        protected override void Dispose(bool disposing) {
            if (disposing) {
                _collection.Dispose();
            }
        }

        private readonly GradientStopCollection _collection;

    }
}
