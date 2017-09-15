using System;
using System.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using FillMode = OpenMLTD.MilliSim.Graphics.Drawing.FillMode;
using Point = System.Drawing.Point;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    public static class Direct2DRenderTargetExtensions {

        public static void FillRectangle(this SharpDX.Direct2D1.RenderTarget target, ID2DBrush brush, float x, float y, float width, float height) {
            var rectF = new RawRectangleF(x, y, x + width, y + height);
            target.FillRectangle(rectF, brush.NativeBrush);
        }

        public static void FillEllipse(this SharpDX.Direct2D1.RenderTarget target, ID2DBrush brush, float x, float y, float width, float height) {
            var ellipse = new Ellipse(new RawVector2(x + width / 2, y + height / 2), width / 2, height / 2);
            target.FillEllipse(ellipse, brush.NativeBrush);
        }

        public static void FillPolygon(this SharpDX.Direct2D1.RenderTarget target, ID2DBrush brush, FillMode fillMode, params PointF[] points) {
            if (points == null) {
                throw new ArgumentNullException(nameof(points));
            }
            if (points.Length == 0) {
                return;
            }
            using (var path = new PathGeometry(target.Factory)) {
                using (var sink = path.Open()) {
                    sink.SetFillMode((SharpDX.Direct2D1.FillMode)fillMode);
                    sink.BeginFigure(new RawVector2(points[0].X, points[0].Y), FigureBegin.Filled);
                    var len = points.Length;
                    for (var i = 1; i < len; ++i) {
                        var pt = points[i];
                        sink.AddLine(new RawVector2(pt.X, pt.Y));
                    }
                    sink.EndFigure(FigureEnd.Closed);
                    sink.Close();
                }
                target.FillGeometry(path, brush.NativeBrush);
            }
        }

        public static void FillPath(this SharpDX.Direct2D1.RenderTarget target, ID2DBrush brush, D2DPathData path) {
            target.FillGeometry(path.NativeGeometry, brush.NativeBrush);
        }

        public static void FillMesh(this SharpDX.Direct2D1.RenderTarget target, ID2DBrush brush, D2DMesh mesh) {
            target.FillMesh(mesh.Native, brush.NativeBrush);
        }

        public static void FillCircle(this SharpDX.Direct2D1.RenderTarget target, ID2DBrush brush, float x, float y, float radius) {
            var w = radius + radius;
            target.FillEllipse(brush, x - radius, y - radius, w, w);
        }

        public static void FillCircle(this SharpDX.Direct2D1.RenderTarget target, ID2DBrush brush, Point center, float radius) {
            var w = radius + radius;
            target.FillEllipse(brush, center.X - radius, center.Y - radius, w, w);
        }

        public static void FillCircle(this SharpDX.Direct2D1.RenderTarget target, ID2DBrush brush, PointF center, float radius) {
            var w = radius + radius;
            target.FillEllipse(brush, center.X - radius, center.Y - radius, w, w);
        }

        public static void DrawLine(this SharpDX.Direct2D1.RenderTarget target, ID2DPen pen, float x1, float y1, float x2, float y2) {
            var pt1 = new RawVector2(x1, y1);
            var pt2 = new RawVector2(x2, y2);
            target.DrawLine(pt1, pt2, pen.Brush.NativeBrush, pen.StrokeWidth, pen.StrokeStyle);
        }

        public static void DrawBezier(this SharpDX.Direct2D1.RenderTarget target, ID2DPen pen, float x1, float y1, float cx1, float cy1, float cx2, float cy2, float x2, float y2) {
            using (var path = new PathGeometry(target.Factory)) {
                using (var sink = path.Open()) {
                    var bezier = new BezierSegment {
                        Point1 = new RawVector2(cx1, cx2),
                        Point2 = new RawVector2(cx2, cy2),
                        Point3 = new RawVector2(x2, y2)
                    };
                    sink.BeginFigure(new RawVector2(x1, y1), FigureBegin.Filled);
                    sink.AddBezier(bezier);
                    sink.EndFigure(FigureEnd.Open);
                    sink.Close();
                }
                target.DrawGeometry(path, pen.Brush.NativeBrush, pen.StrokeWidth, pen.StrokeStyle);
            }
        }

        public static void DrawQuadraticBezier(this SharpDX.Direct2D1.RenderTarget target, ID2DPen pen, float x1, float y1, float cx, float cy, float x2, float y2) {
            using (var path = new PathGeometry(target.Factory)) {
                using (var sink = path.Open()) {
                    var bezier = new QuadraticBezierSegment {
                        Point1 = new RawVector2(cx, cy),
                        Point2 = new RawVector2(x2, y2)
                    };
                    sink.BeginFigure(new RawVector2(x1, y1), FigureBegin.Filled);
                    sink.AddQuadraticBezier(bezier);
                    sink.EndFigure(FigureEnd.Open);
                    sink.Close();
                }
                target.DrawGeometry(path, pen.Brush.NativeBrush, pen.StrokeWidth, pen.StrokeStyle);
            }
        }

        public static void DrawRectangle(this SharpDX.Direct2D1.RenderTarget target, ID2DPen pen, float x, float y, float width, float height) {
            var rect = new RawRectangleF(x, y, x + width, y + height);
            target.DrawRectangle(rect, pen.Brush.NativeBrush, pen.StrokeWidth, pen.StrokeStyle);
        }

        public static void DrawEllipse(this SharpDX.Direct2D1.RenderTarget target, ID2DPen pen, float x, float y, float width, float height) {
            var ellipse = new Ellipse(new RawVector2(x + width / 2, y + height / 2), width / 2, height / 2);
            target.DrawEllipse(ellipse, pen.Brush.NativeBrush, pen.StrokeWidth, pen.StrokeStyle);
        }

        public static void DrawPolygon(this SharpDX.Direct2D1.RenderTarget target, ID2DPen pen, params PointF[] points) {
            if (points == null) {
                throw new ArgumentNullException(nameof(points));
            }
            if (points.Length == 0) {
                return;
            }
            using (var path = new PathGeometry(target.Factory)) {
                using (var sink = path.Open()) {
                    sink.BeginFigure(new RawVector2(points[0].X, points[0].Y), FigureBegin.Filled);
                    var len = points.Length;
                    for (var i = 1; i < len; ++i) {
                        var pt = points[i];
                        sink.AddLine(new RawVector2(pt.X, pt.Y));
                    }
                    sink.EndFigure(FigureEnd.Closed);
                    sink.Close();
                }
                target.DrawGeometry(path, pen.Brush.NativeBrush, pen.StrokeWidth, pen.StrokeStyle);
            }
        }

        public static void DrawPath(this SharpDX.Direct2D1.RenderTarget target, ID2DPen pen, D2DPathData path) {
            target.DrawGeometry(path.NativeGeometry, pen.Brush.NativeBrush, pen.StrokeWidth, pen.StrokeStyle);
        }

        public static void DrawBitmap(this SharpDX.Direct2D1.RenderTarget target, D2DBitmap bitmap, float destX, float destY) {
            target.DrawBitmap(bitmap, destX, destY, bitmap.Width, bitmap.Height);
        }

        public static void DrawBitmap(this SharpDX.Direct2D1.RenderTarget target, D2DBitmap bitmap, float destX, float destY, float destWidth, float destHeight) {
            target.DrawBitmap(bitmap, destX, destY, destWidth, destHeight, BitmapInterpolationMode.Linear);
        }

        public static void DrawBitmap(this SharpDX.Direct2D1.RenderTarget target, D2DBitmap bitmap, float destX, float destY, float destWidth, float destHeight, BitmapInterpolationMode interpolationMode) {
            var destRect = new RawRectangleF(destX, destY, destX + destWidth, destY + destHeight);
            target.DrawBitmap(bitmap.NativeImage, destRect, 1f, interpolationMode);
        }

        public static void DrawBitmap(this SharpDX.Direct2D1.RenderTarget target, D2DBitmap bitmap, float destX, float destY, float destWidth, float destHeight, float srcX, float srcY, float srcWidth, float srcHeight) {
            var destRect = new RawRectangleF(destX, destY, destX + destWidth, destY + destHeight);
            var srcRect = new RawRectangleF(srcX, srcY, srcX + srcWidth, srcY + srcHeight);
            target.DrawBitmap(bitmap.NativeImage, destRect, 1f, BitmapInterpolationMode.Linear, srcRect);
        }

        public static void DrawBitmap(this SharpDX.Direct2D1.RenderTarget target, D2DBitmap bitmap, float destX, float destY, float destWidth, float destHeight, float srcX, float srcY, float srcWidth, float srcHeight, float opacity) {
            var destRect = new RawRectangleF(destX, destY, destX + destWidth, destY + destHeight);
            var srcRect = new RawRectangleF(srcX, srcY, srcX + srcWidth, srcY + srcHeight);
            target.DrawBitmap(bitmap.NativeImage, destRect, opacity, BitmapInterpolationMode.Linear, srcRect);
        }

        public static void DrawText(this SharpDX.Direct2D1.RenderTarget target, string text, ID2DBrush brush, D2DFont font, float destX, float destY, float destWidth, float destHeight) {
            var destRect = new RawRectangleF(destX, destY, destX + destWidth, destY + destHeight);
            target.DrawText(text, font.NativeFont, destRect, brush.NativeBrush);
        }

        public static void DrawCircle(this SharpDX.Direct2D1.RenderTarget target, ID2DPen pen, float x, float y, float radius) {
            var w = radius + radius;
            target.DrawEllipse(pen, x - radius, y - radius, w, w);
        }

        public static void DrawCircle(this SharpDX.Direct2D1.RenderTarget target, ID2DPen pen, Point center, float radius) {
            var w = radius + radius;
            target.DrawEllipse(pen, center.X - radius, center.Y - radius, w, w);
        }

        public static void DrawCircle(this SharpDX.Direct2D1.RenderTarget target, ID2DPen pen, PointF center, float radius) {
            var w = radius + radius;
            target.DrawEllipse(pen, center.X - radius, center.Y - radius, w, w);
        }

        public static void DrawText(this SharpDX.Direct2D1.RenderTarget target, string text, ID2DBrush brush, D2DFont font, float destX, float destY) {
            target.DrawText(text, brush, font, destX, destY, float.MaxValue, float.MaxValue);
        }

        public static void Translate(this SharpDX.Direct2D1.RenderTarget target, float x, float y) {
            target.Transform *= Matrix3x2.Translation(x, y);
        }

        public static void Rotate(this SharpDX.Direct2D1.RenderTarget target, float angle) {
            target.Transform *= Matrix3x2.Rotation(angle);
        }

        public static void ResetTransform(this SharpDX.Direct2D1.RenderTarget target) {
            target.Transform = Matrix3x2.Identity;
        }

    }
}
