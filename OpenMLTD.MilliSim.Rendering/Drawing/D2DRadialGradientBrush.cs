using System;
using System.Drawing;
using System.Linq;
using OpenMLTD.MilliSim.Rendering.Extensions;
using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Rendering.Drawing {
    public sealed class D2DRadialGradientBrush : D2DBrushBase, ID2DBrush {

        public D2DRadialGradientBrush(RenderContext context, PointF center, float radiusX, float radiusY, params Color[] gradientColors)
            : this(context, center, PointF.Empty, radiusX, radiusY, gradientColors) {
        }

        public D2DRadialGradientBrush(RenderContext context, Point center, float radiusX, float radiusY, params Color[] gradientColors)
            : this(context, center, Point.Empty, radiusX, radiusY, gradientColors) {
        }

        public D2DRadialGradientBrush(RenderContext context, PointF center, PointF originOffset, float radiusX, float radiusY, params Color[] gradientColors) {
            if (gradientColors.Length < 2) {
                throw new ArgumentException("Linear gradient brush requires at least 2 colors.", nameof(gradientColors));
            }
            var properties = new RadialGradientBrushProperties {
                Center = center.ToD2DVector(),
                GradientOriginOffset = originOffset.ToD2DVector(),
                RadiusX = radiusX,
                RadiusY = radiusY
            };
            var colorCount = gradientColors.Length;
            var gradientStops = new GradientStop[colorCount];
            for (var i = 0; i < colorCount; ++i) {
                gradientStops[i] = new GradientStop {
                    Color = gradientColors[i].ToRC4(),
                    Position = (float)i / (colorCount - 1)
                };
            }
            var collection = new GradientStopCollection(context.RenderTarget.Direct2DRenderTarget, gradientStops);
            NativeBrush = new RadialGradientBrush(context.RenderTarget.Direct2DRenderTarget, properties, collection);
            _collection = collection;
        }

        public D2DRadialGradientBrush(RenderContext context, Point center, Point originOffset, float radiusX, float radiusY, params Color[] gradientColors) {
            if (gradientColors.Length < 2) {
                throw new ArgumentException("Radial gradient brush requires at least 2 colors.", nameof(gradientColors));
            }
            var properties = new RadialGradientBrushProperties {
                Center = center.ToD2DVector(),
                GradientOriginOffset = originOffset.ToD2DVector(),
                RadiusX = radiusX,
                RadiusY = radiusY
            };
            var colorCount = gradientColors.Length;
            var gradientStops = new GradientStop[colorCount];
            for (var i = 0; i < colorCount; ++i) {
                gradientStops[i] = new GradientStop {
                    Color = gradientColors[i].ToRC4(),
                    Position = (float)i / (colorCount - 1)
                };
            }
            var collection = new GradientStopCollection(context.RenderTarget.Direct2DRenderTarget, gradientStops);
            NativeBrush = new RadialGradientBrush(context.RenderTarget.Direct2DRenderTarget, properties, collection);
            _collection = collection;
        }

        public D2DRadialGradientBrush(RenderContext context, PointF center, float radiusX, float radiusY, params (Color Color, float Position)[] gradientStops) {
            var properties = new RadialGradientBrushProperties {
                Center = center.ToD2DVector(),
                RadiusX = radiusX,
                RadiusY = radiusY
            };
            var collection = new GradientStopCollection(context.RenderTarget.Direct2DRenderTarget, gradientStops.Select(t => new GradientStop {
                Color = t.Color.ToRC4(),
                Position = t.Position
            }).ToArray());
            NativeBrush = new RadialGradientBrush(context.RenderTarget.Direct2DRenderTarget, properties, collection);
            _collection = collection;
        }

        public D2DRadialGradientBrush(RenderContext context, Point center, float radiusX, float radiusY, params (Color Color, float Position)[] gradientStops) {
            var properties = new RadialGradientBrushProperties {
                Center = center.ToD2DVector(),
                RadiusX = radiusX,
                RadiusY = radiusY
            };
            var collection = new GradientStopCollection(context.RenderTarget.Direct2DRenderTarget, gradientStops.Select(t => new GradientStop {
                Color = t.Color.ToRC4(),
                Position = t.Position
            }).ToArray());
            NativeBrush = new RadialGradientBrush(context.RenderTarget.Direct2DRenderTarget, properties, collection);
            _collection = collection;
        }

        public D2DRadialGradientBrush(RenderContext context, PointF center, float radiusX, float radiusY, params GradientStop[] gradientStops)
            : this(context, center, PointF.Empty, radiusX, radiusY, gradientStops) {
        }

        public D2DRadialGradientBrush(RenderContext context, Point center, float radiusX, float radiusY, params GradientStop[] gradientStops)
            : this(context, center, Point.Empty, radiusX, radiusY, gradientStops) {
        }

        public D2DRadialGradientBrush(RenderContext context, PointF center, PointF originOffset, float radiusX, float radiusY, params GradientStop[] gradientStops) {
            var properties = new RadialGradientBrushProperties {
                Center = center.ToD2DVector(),
                GradientOriginOffset = originOffset.ToD2DVector(),
                RadiusX = radiusX,
                RadiusY = radiusY
            };
            var collection = new GradientStopCollection(context.RenderTarget.Direct2DRenderTarget, gradientStops);
            NativeBrush = new RadialGradientBrush(context.RenderTarget.Direct2DRenderTarget, properties, collection);
            _collection = collection;
        }

        public D2DRadialGradientBrush(RenderContext context, Point center, Point originOffset, float radiusX, float radiusY, params GradientStop[] gradientStops) {
            var properties = new RadialGradientBrushProperties {
                Center = center.ToD2DVector(),
                GradientOriginOffset = originOffset.ToD2DVector(),
                RadiusX = radiusX,
                RadiusY = radiusY
            };
            var collection = new GradientStopCollection(context.RenderTarget.Direct2DRenderTarget, gradientStops);
            NativeBrush = new RadialGradientBrush(context.RenderTarget.Direct2DRenderTarget, properties, collection);
            _collection = collection;
        }

        public D2DRadialGradientBrush(RenderContext context, PointF center, float radiusX, float radiusY, GradientStopCollection collection)
            : this(context, center, PointF.Empty, radiusX, radiusY, collection) {
        }

        public D2DRadialGradientBrush(RenderContext context, Point center, float radiusX, float radiusY, GradientStopCollection collection)
            : this(context, center, Point.Empty, radiusX, radiusY, collection) {
        }

        public D2DRadialGradientBrush(RenderContext context, PointF center, PointF originOffset, float radiusX, float radiusY, GradientStopCollection collection) {
            var properties = new RadialGradientBrushProperties {
                Center = center.ToD2DVector(),
                GradientOriginOffset = originOffset.ToD2DVector(),
                RadiusX = radiusX,
                RadiusY = radiusY
            };
            NativeBrush = new RadialGradientBrush(context.RenderTarget.Direct2DRenderTarget, properties, collection);
            _collection = collection;
        }

        public D2DRadialGradientBrush(RenderContext context, Point center, Point originOffset, float radiusX, float radiusY, GradientStopCollection collection) {
            var properties = new RadialGradientBrushProperties {
                Center = center.ToD2DVector(),
                GradientOriginOffset = originOffset.ToD2DVector(),
                RadiusX = radiusX,
                RadiusY = radiusY
            };
            NativeBrush = new RadialGradientBrush(context.RenderTarget.Direct2DRenderTarget, properties, collection);
            _collection = collection;
        }

        public D2DRadialGradientBrush(RenderContext context, RadialGradientBrushProperties properties, GradientStopCollection collection) {
            NativeBrush = new RadialGradientBrush(context.RenderTarget.Direct2DRenderTarget, properties, collection);
            _collection = collection;
        }

        public RadialGradientBrush NativeBrush { get; }

        SharpDX.Direct2D1.Brush ID2DBrush.NativeBrush => NativeBrush;

        protected override void Dispose(bool disposing) {
            if (disposing) {
                NativeBrush.Dispose();
                _collection.Dispose();
            }
        }

        private readonly GradientStopCollection _collection;

    }
}
