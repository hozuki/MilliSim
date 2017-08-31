using System.Drawing;
using OpenMLTD.MilliSim.Rendering.Drawing;
using SharpDX.Direct2D1;
using SharpDX;
using Point = System.Drawing.Point;

namespace OpenMLTD.MilliSim.Rendering.Extensions {
    public static partial class RenderContextExtensions {

        public static void FillRectangle(this RenderContext context, ID2DBrush brush, float x, float y, float width, float height) {
            context.RenderTarget.DeviceContext.FillRectangle(brush, x, y, width, height);
        }

        public static void FillEllipse(this RenderContext context, ID2DBrush brush, float x, float y, float width, float height) {
            context.RenderTarget.DeviceContext.FillEllipse(brush, x, y, width, height);
        }

        public static void FillPolygon(this RenderContext context, ID2DBrush brush, FillMode fillMode, params PointF[] points) {
            context.RenderTarget.DeviceContext.FillPolygon(brush, fillMode, points);
        }

        public static void FillPath(this RenderContext context, ID2DBrush brush, D2DPathData path) {
            context.RenderTarget.DeviceContext.FillPath(brush, path);
        }

        public static void FillPath(this RenderContext context, ID2DBrush brush, D2DPathData path, float offsetX, float offsetY) {
            context.PushTransform();
            context.Translate(offsetX, offsetY);
            context.RenderTarget.DeviceContext.FillPath(brush, path);
            context.PopTransform();
        }

        public static void FillPath(this RenderContext context, ID2DBrush brush, D2DPathData path, Point offset) {
            context.FillPath(brush, path, offset.X, offset.Y);
        }

        public static void FillPath(this RenderContext context, ID2DBrush brush, D2DPathData path, PointF offset) {
            context.FillPath(brush, path, offset.X, offset.Y);
        }

        public static void FillMesh(this RenderContext context, ID2DBrush brush, D2DMesh mesh) {
            context.RenderTarget.DeviceContext.FillMesh(brush, mesh);
        }

        public static void FillCircle(this RenderContext context, ID2DBrush brush, float x, float y, float radius) {
            context.RenderTarget.DeviceContext.FillCircle(brush, x, y, radius);
        }

        public static void FillCircle(this RenderContext context, ID2DBrush brush, Point center, float radius) {
            context.RenderTarget.DeviceContext.FillCircle(brush, center, radius);
        }

        public static void FillCircle(this RenderContext context, ID2DBrush brush, PointF center, float radius) {
            context.RenderTarget.DeviceContext.FillCircle(brush, center, radius);
        }

        public static void DrawLine(this RenderContext context, ID2DPen pen, float x1, float y1, float x2, float y2) {
            context.RenderTarget.DeviceContext.DrawLine(pen, x1, y1, x2, y2);
        }

        public static void DrawBezier(this RenderContext context, ID2DPen pen, float x1, float y1, float cx1, float cy1, float cx2, float cy2, float x2, float y2) {
            context.RenderTarget.DeviceContext.DrawBezier(pen, x1, y1, cx1, cy1, cx2, cy2, x2, y2);
        }

        public static void DrawQuadraticBezier(this RenderContext context, ID2DPen pen, float x1, float y1, float cx, float cy, float x2, float y2) {
            context.RenderTarget.DeviceContext.DrawQuadraticBezier(pen, x1, y1, cx, cy, x2, y2);
        }

        public static void DrawRectangle(this RenderContext context, ID2DPen pen, float x, float y, float width, float height) {
            context.RenderTarget.DeviceContext.DrawRectangle(pen, x, y, width, height);
        }

        public static void DrawEllipse(this RenderContext context, ID2DPen pen, float x, float y, float width, float height) {
            context.RenderTarget.DeviceContext.DrawEllipse(pen, x, y, width, height);
        }

        public static void DrawPolygon(this RenderContext context, ID2DPen pen, params PointF[] points) {
            context.RenderTarget.DeviceContext.DrawPolygon(pen, points);
        }

        public static void DrawPath(this RenderContext context, ID2DPen pen, D2DPathData path) {
            context.RenderTarget.DeviceContext.DrawPath(pen, path);
        }

        public static void DrawPath(this RenderContext context, ID2DPen pen, D2DPathData path, float offsetX, float offsetY) {
            context.PushTransform();
            context.Translate(offsetX, offsetY);
            context.RenderTarget.DeviceContext.DrawPath(pen, path);
            context.PopTransform();
        }

        public static void DrawPath(this RenderContext context, ID2DPen pen, D2DPathData path, Point offset) {
            context.DrawPath(pen, path, offset.X, offset.Y);
        }

        public static void DrawPath(this RenderContext context, ID2DPen pen, D2DPathData path, PointF offset) {
            context.DrawPath(pen, path, offset.X, offset.Y);
        }

        public static void DrawImage(this RenderContext context, ID2DImage image, float destX, float destY) {
            context.RenderTarget.DeviceContext.DrawImage(image, destX, destY);
        }

        public static void DrawImage(this RenderContext context, ID2DImage image, float destX, float destY, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            context.RenderTarget.DeviceContext.DrawImage(image, destX, destY, interpolationMode, compositeMode);
        }

        public static void DrawImage(this RenderContext context, ID2DImage image, float destX, float destY, float srcX, float srcY, float srcWidth, float srcHeight) {
            context.RenderTarget.DeviceContext.DrawImage(image, destX, destY, srcX, srcY, srcWidth, srcHeight);
        }

        public static void DrawImage(this RenderContext context, ID2DImage image, float destX, float destY, float srcX, float srcY, float srcWidth, float srcHeight, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            context.RenderTarget.DeviceContext.DrawImage(image, destX, destY, srcX, srcY, srcWidth, srcHeight, interpolationMode, compositeMode);
        }

        public static void DrawImage(this RenderContext context, D2DEffect effect) {
            context.RenderTarget.DeviceContext.DrawImage(effect);
        }

        public static void DrawImage(this RenderContext context, D2DEffect effect, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            context.RenderTarget.DeviceContext.DrawImage(effect, interpolationMode, compositeMode);
        }

        public static void DrawImage(this RenderContext context, D2DEffect effect, float destX, float destY) {
            context.RenderTarget.DeviceContext.DrawImage(effect, destX, destY);
        }

        public static void DrawImage(this RenderContext context, D2DEffect effect, float destX, float destY, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            context.RenderTarget.DeviceContext.DrawImage(effect, destX, destY, interpolationMode, compositeMode);
        }

        public static void DrawBitmap(this RenderContext context, D2DBitmap bitmap, float destX, float destY) {
            context.RenderTarget.DeviceContext.DrawBitmap(bitmap, destX, destY, bitmap.Width, bitmap.Height);
        }

        public static void DrawBitmap(this RenderContext context, D2DBitmap bitmap, float destX, float destY, float destWidth, float destHeight) {
            context.RenderTarget.DeviceContext.DrawBitmap(bitmap, destX, destY, destWidth, destHeight);
        }

        public static void DrawBitmap(this RenderContext context, D2DBitmap bitmap, float destX, float destY, float destWidth, float destHeight, BitmapInterpolationMode interpolationMode) {
            context.RenderTarget.DeviceContext.DrawBitmap(bitmap, destX, destY, destWidth, destHeight, interpolationMode);
        }

        public static void DrawBitmap(this RenderContext context, D2DBitmap bitmap, float destX, float destY, float destWidth, float destHeight, float srcX, float srcY, float srcWidth, float srcHeight) {
            context.RenderTarget.DeviceContext.DrawBitmap(bitmap, destX, destY, destWidth, destHeight, srcX, srcY, srcWidth, srcHeight);
        }

        public static void DrawText(this RenderContext context, string text, ID2DBrush brush, D2DFont font, float destX, float destY, float destWidth, float destHeight) {
            context.RenderTarget.DeviceContext.DrawText(text, brush, font, destX, destY, destWidth, destHeight);
        }

        public static SizeF MeasureText(this RenderContext context, string text, D2DFont font) {
            return DirectWriteHelper.MeasureText(context.DirectWriteFactory, text, font);
        }

        public static SizeF MeasureText(this RenderContext context, string text, D2DFont font, float maxWidth, float maxHeight) {
            return DirectWriteHelper.MeasureText(context.DirectWriteFactory, text, font, maxWidth, maxHeight);
        }

        public static void DrawCircle(this RenderContext context, ID2DPen pen, float x, float y, float radius) {
            context.RenderTarget.DeviceContext.DrawCircle(pen, x, y, radius);
        }

        public static void DrawCircle(this RenderContext context, ID2DPen pen, Point center, float radius) {
            context.RenderTarget.DeviceContext.DrawCircle(pen, center, radius);
        }

        public static void DrawCircle(this RenderContext context, ID2DPen pen, PointF center, float radius) {
            context.RenderTarget.DeviceContext.DrawCircle(pen, center, radius);
        }

        public static void DrawText(this RenderContext context, string text, ID2DBrush brush, D2DFont font, float destX, float destY) {
            context.RenderTarget.DeviceContext.DrawText(text, brush, font, destX, destY);
        }

        public static void Translate(this RenderContext context, float x, float y) {
            context.RenderTarget.DeviceContext.Translate(x, y);
        }

        public static void Rotate(this RenderContext context, float angle) {
            context.RenderTarget.DeviceContext.Rotate(angle);
        }

        public static void ResetTransform(this RenderContext context) {
            context.RenderTarget.DeviceContext.ResetTransform();
        }

        public static void PushTransform(this RenderContext context) {
            context.RenderTarget.PushTransform();
        }

        public static Matrix3x2 PopTransform(this RenderContext context) {
            return context.RenderTarget.PopTransform();
        }

    }
}
