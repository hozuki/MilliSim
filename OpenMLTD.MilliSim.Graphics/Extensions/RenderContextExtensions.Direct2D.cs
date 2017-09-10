using System.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D;
using SharpDX;
using SharpDX.Direct2D1;
using FillMode = OpenMLTD.MilliSim.Graphics.Drawing.FillMode;
using Point = System.Drawing.Point;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    partial class RenderContextExtensions {

        public static void FillRectangle(this RenderContext context, ID2DBrush brush, float x, float y, float width, float height) {
            context.RenderTarget.DeviceContext2D.FillRectangle(brush, x, y, width, height);
        }

        public static void FillEllipse(this RenderContext context, ID2DBrush brush, float x, float y, float width, float height) {
            context.RenderTarget.DeviceContext2D.FillEllipse(brush, x, y, width, height);
        }

        public static void FillPolygon(this RenderContext context, ID2DBrush brush, FillMode fillMode, params PointF[] points) {
            context.RenderTarget.DeviceContext2D.FillPolygon(brush, fillMode, points);
        }

        public static void FillPath(this RenderContext context, ID2DBrush brush, D2DPathData path) {
            context.RenderTarget.DeviceContext2D.FillPath(brush, path);
        }

        public static void FillPath(this RenderContext context, ID2DBrush brush, D2DPathData path, float offsetX, float offsetY) {
            context.PushTransform2D();
            context.Translate2D(offsetX, offsetY);
            context.RenderTarget.DeviceContext2D.FillPath(brush, path);
            context.PopTransform2D();
        }

        public static void FillPath(this RenderContext context, ID2DBrush brush, D2DPathData path, Point offset) {
            context.FillPath(brush, path, offset.X, offset.Y);
        }

        public static void FillPath(this RenderContext context, ID2DBrush brush, D2DPathData path, PointF offset) {
            context.FillPath(brush, path, offset.X, offset.Y);
        }

        public static void FillMesh(this RenderContext context, ID2DBrush brush, D2DMesh mesh) {
            context.RenderTarget.DeviceContext2D.FillMesh(brush, mesh);
        }

        public static void FillCircle(this RenderContext context, ID2DBrush brush, float x, float y, float radius) {
            context.RenderTarget.DeviceContext2D.FillCircle(brush, x, y, radius);
        }

        public static void FillCircle(this RenderContext context, ID2DBrush brush, Point center, float radius) {
            context.RenderTarget.DeviceContext2D.FillCircle(brush, center, radius);
        }

        public static void FillCircle(this RenderContext context, ID2DBrush brush, PointF center, float radius) {
            context.RenderTarget.DeviceContext2D.FillCircle(brush, center, radius);
        }

        public static void DrawLine(this RenderContext context, ID2DPen pen, float x1, float y1, float x2, float y2) {
            context.RenderTarget.DeviceContext2D.DrawLine(pen, x1, y1, x2, y2);
        }

        public static void DrawBezier(this RenderContext context, ID2DPen pen, float x1, float y1, float cx1, float cy1, float cx2, float cy2, float x2, float y2) {
            context.RenderTarget.DeviceContext2D.DrawBezier(pen, x1, y1, cx1, cy1, cx2, cy2, x2, y2);
        }

        public static void DrawQuadraticBezier(this RenderContext context, ID2DPen pen, float x1, float y1, float cx, float cy, float x2, float y2) {
            context.RenderTarget.DeviceContext2D.DrawQuadraticBezier(pen, x1, y1, cx, cy, x2, y2);
        }

        public static void DrawRectangle(this RenderContext context, ID2DPen pen, float x, float y, float width, float height) {
            context.RenderTarget.DeviceContext2D.DrawRectangle(pen, x, y, width, height);
        }

        public static void DrawEllipse(this RenderContext context, ID2DPen pen, float x, float y, float width, float height) {
            context.RenderTarget.DeviceContext2D.DrawEllipse(pen, x, y, width, height);
        }

        public static void DrawPolygon(this RenderContext context, ID2DPen pen, params PointF[] points) {
            context.RenderTarget.DeviceContext2D.DrawPolygon(pen, points);
        }

        public static void DrawPath(this RenderContext context, ID2DPen pen, D2DPathData path) {
            context.RenderTarget.DeviceContext2D.DrawPath(pen, path);
        }

        public static void DrawPath(this RenderContext context, ID2DPen pen, D2DPathData path, float offsetX, float offsetY) {
            context.PushTransform2D();
            context.Translate2D(offsetX, offsetY);
            context.RenderTarget.DeviceContext2D.DrawPath(pen, path);
            context.PopTransform2D();
        }

        public static void DrawPath(this RenderContext context, ID2DPen pen, D2DPathData path, Point offset) {
            context.DrawPath(pen, path, offset.X, offset.Y);
        }

        public static void DrawPath(this RenderContext context, ID2DPen pen, D2DPathData path, PointF offset) {
            context.DrawPath(pen, path, offset.X, offset.Y);
        }

        public static void DrawImage(this RenderContext context, ID2DImage image, float destX, float destY) {
            context.RenderTarget.DeviceContext2D.DrawImage(image, destX, destY);
        }

        public static void DrawImage(this RenderContext context, ID2DImage image, float destX, float destY, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            context.RenderTarget.DeviceContext2D.DrawImage(image, destX, destY, interpolationMode, compositeMode);
        }

        public static void DrawImage(this RenderContext context, ID2DImage image, float destX, float destY, float srcX, float srcY, float srcWidth, float srcHeight) {
            context.RenderTarget.DeviceContext2D.DrawImage(image, destX, destY, srcX, srcY, srcWidth, srcHeight);
        }

        public static void DrawImage(this RenderContext context, ID2DImage image, float destX, float destY, float srcX, float srcY, float srcWidth, float srcHeight, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            context.RenderTarget.DeviceContext2D.DrawImage(image, destX, destY, srcX, srcY, srcWidth, srcHeight, interpolationMode, compositeMode);
        }

        public static void DrawImage(this RenderContext context, D2DEffect effect) {
            context.RenderTarget.DeviceContext2D.DrawImage(effect);
        }

        public static void DrawImage(this RenderContext context, D2DEffect effect, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            context.RenderTarget.DeviceContext2D.DrawImage(effect, interpolationMode, compositeMode);
        }

        public static void DrawImage(this RenderContext context, D2DEffect effect, float destX, float destY) {
            context.RenderTarget.DeviceContext2D.DrawImage(effect, destX, destY);
        }

        public static void DrawImage(this RenderContext context, D2DEffect effect, float destX, float destY, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            context.RenderTarget.DeviceContext2D.DrawImage(effect, destX, destY, interpolationMode, compositeMode);
        }

        public static void DrawBitmap(this RenderContext context, D2DBitmap bitmap, float destX, float destY) {
            context.RenderTarget.DeviceContext2D.DrawBitmap(bitmap, destX, destY, bitmap.Width, bitmap.Height);
        }

        public static void DrawBitmap(this RenderContext context, D2DBitmap bitmap, float destX, float destY, float destWidth, float destHeight) {
            context.RenderTarget.DeviceContext2D.DrawBitmap(bitmap, destX, destY, destWidth, destHeight);
        }

        public static void DrawBitmap(this RenderContext context, D2DBitmap bitmap, float destX, float destY, float destWidth, float destHeight, BitmapInterpolationMode interpolationMode) {
            context.RenderTarget.DeviceContext2D.DrawBitmap(bitmap, destX, destY, destWidth, destHeight, interpolationMode);
        }

        public static void DrawBitmap(this RenderContext context, D2DBitmap bitmap, float destX, float destY, float destWidth, float destHeight, float srcX, float srcY, float srcWidth, float srcHeight) {
            context.RenderTarget.DeviceContext2D.DrawBitmap(bitmap, destX, destY, destWidth, destHeight, srcX, srcY, srcWidth, srcHeight);
        }

        public static void DrawBitmap(this RenderContext context, D2DBitmap bitmap, float destX, float destY, float destWidth, float destHeight, float srcX, float srcY, float srcWidth, float srcHeight, float opacity) {
            context.RenderTarget.DeviceContext2D.DrawBitmap(bitmap, destX, destY, destWidth, destHeight, srcX, srcY, srcWidth, srcHeight, opacity);
        }

        public static void DrawText(this RenderContext context, string text, ID2DBrush brush, D2DFont font, float destX, float destY, float destWidth, float destHeight) {
            context.RenderTarget.DeviceContext2D.DrawText(text, brush, font, destX, destY, destWidth, destHeight);
        }

        public static SizeF MeasureText(this RenderContext context, string text, D2DFont font) {
            return DirectWriteHelper.MeasureText(context.DirectWriteFactory, text, font);
        }

        public static SizeF MeasureText(this RenderContext context, string text, D2DFont font, float maxWidth, float maxHeight) {
            return DirectWriteHelper.MeasureText(context.DirectWriteFactory, text, font, maxWidth, maxHeight);
        }

        public static void DrawCircle(this RenderContext context, ID2DPen pen, float x, float y, float radius) {
            context.RenderTarget.DeviceContext2D.DrawCircle(pen, x, y, radius);
        }

        public static void DrawCircle(this RenderContext context, ID2DPen pen, Point center, float radius) {
            context.RenderTarget.DeviceContext2D.DrawCircle(pen, center, radius);
        }

        public static void DrawCircle(this RenderContext context, ID2DPen pen, PointF center, float radius) {
            context.RenderTarget.DeviceContext2D.DrawCircle(pen, center, radius);
        }

        public static void DrawText(this RenderContext context, string text, ID2DBrush brush, D2DFont font, float destX, float destY) {
            context.RenderTarget.DeviceContext2D.DrawText(text, brush, font, destX, destY);
        }

        public static void Translate2D(this RenderContext context, float x, float y) {
            context.RenderTarget.DeviceContext2D.Translate(x, y);
        }

        public static void Rotate2D(this RenderContext context, float angle) {
            context.RenderTarget.DeviceContext2D.Rotate(angle);
        }

        public static void ResetTransform2D(this RenderContext context) {
            context.RenderTarget.DeviceContext2D.ResetTransform();
        }

        public static void PushTransform2D(this RenderContext context) {
            context.RenderTarget.PushTransform();
        }

        public static Matrix3x2 PopTransform2D(this RenderContext context) {
            return context.RenderTarget.PopTransform();
        }

    }
}
