using System.Drawing;
using OpenMLTD.MilliSim.Rendering.Drawing;
using OpenMLTD.MilliSim.Rendering.Drawing.Advanced;
using SharpDX.Direct2D1;
using SharpDX;
using Point = System.Drawing.Point;

namespace OpenMLTD.MilliSim.Rendering.Extensions {
    public static partial class RenderContextExtensions {

        public static void FillRectangle(this RenderContext context, ID2DBrush brush, float x, float y, float width, float height) {
            context.RenderTarget.Direct2DRenderTarget.FillRectangle(brush, x, y, width, height);
        }

        public static void FillEllipse(this RenderContext context, ID2DBrush brush, float x, float y, float width, float height) {
            context.RenderTarget.Direct2DRenderTarget.FillEllipse(brush, x, y, width, height);
        }

        public static void FillPolygon(this RenderContext context, ID2DBrush brush, FillMode fillMode, params PointF[] points) {
            context.RenderTarget.Direct2DRenderTarget.FillPolygon(brush, fillMode, points);
        }

        public static void FillPath(this RenderContext context, ID2DBrush brush, D2DPathData path) {
            context.RenderTarget.Direct2DRenderTarget.FillPath(brush, path);
        }

        public static void FillPath(this RenderContext context, ID2DBrush brush, D2DPathData path, float offsetX, float offsetY) {
            context.PushTransform();
            context.Translate(offsetX, offsetY);
            context.RenderTarget.Direct2DRenderTarget.FillPath(brush, path);
            context.PopTransform();
        }

        public static void FillPath(this RenderContext context, ID2DBrush brush, D2DPathData path, Point offset) {
            context.FillPath(brush, path, offset.X, offset.Y);
        }

        public static void FillPath(this RenderContext context, ID2DBrush brush, D2DPathData path, PointF offset) {
            context.FillPath(brush, path, offset.X, offset.Y);
        }

        public static void FillPath(this RenderContext context, ID2DBrush brush, D2DFontPathData path) {
            path.Fill(context, brush);
        }

        /// <summary>
        /// Fills a <see cref="D2DFontPathData"/> to current <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="RenderContext"/> to fill to.</param>
        /// <param name="brush">The <see cref="D2DBrushBase"/> to use.</param>
        /// <param name="path">The <see cref="D2DFontPathData"/> to fill.</param>
        /// <param name="offsetX">The X offset on target <see cref="RenderContext"/>.</param>
        /// <param name="offsetY">The Y offset on target <see cref="RenderContext"/>.</param>
        /// <param name="yCorrection">If <see langword="true"/>, apply automatic Y correction: y(real) = offsetY + lineHeight. If <see langword="false"/>, the Y value stays untouched.</param>
        public static void FillPath(this RenderContext context, ID2DBrush brush, D2DFontPathData path, float offsetX, float offsetY, bool yCorrection) {
            if (yCorrection) {
                offsetY += path.LineHeight;
            }
            path.Fill(context, brush, offsetX, offsetY);
        }

        public static void FillPath(this RenderContext context, ID2DBrush brush, D2DFontPathData path, float offsetX, float offsetY) {
            context.FillPath(brush, path, offsetX, offsetY, true);
        }

        public static void FillPath(this RenderContext context, ID2DBrush brush, D2DFontPathData path, Point offset, bool yCorrection) {
            context.FillPath(brush, path, offset.X, offset.Y, yCorrection);
        }

        public static void FillPath(this RenderContext context, ID2DBrush brush, D2DFontPathData path, Point offset) {
            context.FillPath(brush, path, offset, true);
        }

        public static void FillPath(this RenderContext context, ID2DBrush brush, D2DFontPathData path, PointF offset, bool yCorrection) {
            context.FillPath(brush, path, offset.X, offset.Y, yCorrection);
        }

        public static void FillPath(this RenderContext context, ID2DBrush brush, D2DFontPathData path, PointF offset) {
            context.FillPath(brush, path, offset, true);
        }

        public static void FillMesh(this RenderContext context, ID2DBrush brush, D2DMesh mesh) {
            context.RenderTarget.Direct2DRenderTarget.FillMesh(brush, mesh);
        }

        public static void FillCircle(this RenderContext context, ID2DBrush brush, float x, float y, float radius) {
            context.RenderTarget.Direct2DRenderTarget.FillCircle(brush, x, y, radius);
        }

        public static void FillCircle(this RenderContext context, ID2DBrush brush, Point center, float radius) {
            context.RenderTarget.Direct2DRenderTarget.FillCircle(brush, center, radius);
        }

        public static void FillCircle(this RenderContext context, ID2DBrush brush, PointF center, float radius) {
            context.RenderTarget.Direct2DRenderTarget.FillCircle(brush, center, radius);
        }

        public static void DrawLine(this RenderContext context, ID2DPen pen, float x1, float y1, float x2, float y2) {
            context.RenderTarget.Direct2DRenderTarget.DrawLine(pen, x1, y1, x2, y2);
        }

        public static void DrawBezier(this RenderContext context, ID2DPen pen, float x1, float y1, float cx1, float cy1, float cx2, float cy2, float x2, float y2) {
            context.RenderTarget.Direct2DRenderTarget.DrawBezier(pen, x1, y1, cx1, cy1, cx2, cy2, x2, y2);
        }

        public static void DrawQuadraticBezier(this RenderContext context, ID2DPen pen, float x1, float y1, float cx, float cy, float x2, float y2) {
            context.RenderTarget.Direct2DRenderTarget.DrawQuadraticBezier(pen, x1, y1, cx, cy, x2, y2);
        }

        public static void DrawRectangle(this RenderContext context, ID2DPen pen, float x, float y, float width, float height) {
            context.RenderTarget.Direct2DRenderTarget.DrawRectangle(pen, x, y, width, height);
        }

        public static void DrawEllipse(this RenderContext context, ID2DPen pen, float x, float y, float width, float height) {
            context.RenderTarget.Direct2DRenderTarget.DrawEllipse(pen, x, y, width, height);
        }

        public static void DrawPolygon(this RenderContext context, ID2DPen pen, params PointF[] points) {
            context.RenderTarget.Direct2DRenderTarget.DrawPolygon(pen, points);
        }

        public static void DrawPath(this RenderContext context, ID2DPen pen, D2DPathData path) {
            context.RenderTarget.Direct2DRenderTarget.DrawPath(pen, path);
        }

        public static void DrawPath(this RenderContext context, ID2DPen pen, D2DPathData path, float offsetX, float offsetY) {
            context.PushTransform();
            context.Translate(offsetX, offsetY);
            context.RenderTarget.Direct2DRenderTarget.DrawPath(pen, path);
            context.PopTransform();
        }

        public static void DrawPath(this RenderContext context, ID2DPen pen, D2DPathData path, Point offset) {
            context.DrawPath(pen, path, offset.X, offset.Y);
        }

        public static void DrawPath(this RenderContext context, ID2DPen pen, D2DPathData path, PointF offset) {
            context.DrawPath(pen, path, offset.X, offset.Y);
        }

        public static void DrawPath(this RenderContext context, ID2DPen pen, D2DFontPathData path) {
            path.Draw(context, pen);
        }

        /// <summary>
        /// Draws a <see cref="D2DFontPathData"/> to current <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="RenderContext"/> to draw to.</param>
        /// <param name="pen">The <see cref="D2DPen"/> to use.</param>
        /// <param name="path">The <see cref="D2DFontPathData"/> to draw.</param>
        /// <param name="offsetX">The X offset on target <see cref="RenderContext"/>.</param>
        /// <param name="offsetY">The Y offset on target <see cref="RenderContext"/>.</param>
        /// <param name="yCorrection">If <see langword="true"/>, apply automatic Y correction: y(real) = offsetY + lineHeight. If <see langword="false"/>, the Y value stays untouched.</param>
        public static void DrawPath(this RenderContext context, ID2DPen pen, D2DFontPathData path, float offsetX, float offsetY, bool yCorrection) {
            if (yCorrection) {
                offsetY += path.LineHeight;
            }
            path.Draw(context, pen, offsetX, offsetY);
        }

        public static void DrawPath(this RenderContext context, ID2DPen pen, D2DFontPathData path, float offsetX, float offsetY) {
            context.DrawPath(pen, path, offsetX, offsetY, true);
        }

        public static void DrawPath(this RenderContext context, ID2DPen pen, D2DFontPathData path, Point offset, bool yCorrection) {
            context.DrawPath(pen, path, offset.X, offset.Y, yCorrection);
        }

        public static void DrawPath(this RenderContext context, ID2DPen pen, D2DFontPathData path, Point offset) {
            context.DrawPath(pen, path, offset, true);
        }

        public static void DrawPath(this RenderContext context, ID2DPen pen, D2DFontPathData path, PointF offset, bool yCorrection) {
            context.DrawPath(pen, path, offset.X, offset.Y, yCorrection);
        }

        public static void DrawPath(this RenderContext context, ID2DPen pen, D2DFontPathData path, PointF offset) {
            context.DrawPath(pen, path, offset, true);
        }

        public static void DrawImage(this RenderContext context, ID2DImage image, float destX, float destY) {
            context.Direct2DDeviceContext.DrawImage(image, destX, destY);
        }

        public static void DrawImage(this RenderContext context, ID2DImage image, float destX, float destY, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            context.Direct2DDeviceContext.DrawImage(image, destX, destY, interpolationMode, compositeMode);
        }

        public static void DrawImage(this RenderContext context, ID2DImage image, float destX, float destY, float srcX, float srcY, float srcWidth, float srcHeight) {
            context.Direct2DDeviceContext.DrawImage(image, destX, destY, srcX, srcY, srcWidth, srcHeight);
        }

        public static void DrawImage(this RenderContext context, ID2DImage image, float destX, float destY, float srcX, float srcY, float srcWidth, float srcHeight, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            context.Direct2DDeviceContext.DrawImage(image, destX, destY, srcX, srcY, srcWidth, srcHeight, interpolationMode, compositeMode);
        }

        public static void DrawImage(this RenderContext context, D2DEffect effect) {
            context.Direct2DDeviceContext.DrawImage(effect);
        }

        public static void DrawImage(this RenderContext context, D2DEffect effect, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            context.Direct2DDeviceContext.DrawImage(effect, interpolationMode, compositeMode);
        }

        public static void DrawImage(this RenderContext context, D2DEffect effect, float destX, float destY) {
            context.Direct2DDeviceContext.DrawImage(effect, destX, destY);
        }

        public static void DrawImage(this RenderContext context, D2DEffect effect, float destX, float destY, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            context.Direct2DDeviceContext.DrawImage(effect, destX, destY, interpolationMode, compositeMode);
        }

        public static void DrawBitmap(this RenderContext context, D2DBitmap bitmap, float destX, float destY) {
            context.RenderTarget.Direct2DRenderTarget.DrawBitmap(bitmap, destX, destY, bitmap.Width, bitmap.Height);
        }

        public static void DrawBitmap(this RenderContext context, D2DBitmap bitmap, float destX, float destY, float destWidth, float destHeight) {
            context.RenderTarget.Direct2DRenderTarget.DrawBitmap(bitmap, destX, destY, destWidth, destHeight);
        }

        public static void DrawBitmap(this RenderContext context, D2DBitmap bitmap, float destX, float destY, float destWidth, float destHeight, float srcX, float srcY, float srcWidth, float srcHeight) {
            context.RenderTarget.Direct2DRenderTarget.DrawBitmap(bitmap, destX, destY, destWidth, destHeight, srcX, srcY, srcWidth, srcHeight);
        }

        public static void DrawText(this RenderContext context, string text, ID2DBrush brush, D2DFont font, float destX, float destY, float destWidth, float destHeight) {
            context.RenderTarget.Direct2DRenderTarget.DrawText(text, brush, font, destX, destY, destWidth, destHeight);
        }

        public static SizeF MeasureText(this RenderContext context, string text, D2DFont font) {
            return DirectWriteHelper.MeasureText(context.DirectWriteFactory, text, font);
        }

        public static SizeF MeasureText(this RenderContext context, string text, D2DFont font, float maxWidth, float maxHeight) {
            return DirectWriteHelper.MeasureText(context.DirectWriteFactory, text, font, maxWidth, maxHeight);
        }

        public static void DrawCircle(this RenderContext context, ID2DPen pen, float x, float y, float radius) {
            context.RenderTarget.Direct2DRenderTarget.DrawCircle(pen, x, y, radius);
        }

        public static void DrawCircle(this RenderContext context, ID2DPen pen, Point center, float radius) {
            context.RenderTarget.Direct2DRenderTarget.DrawCircle(pen, center, radius);
        }

        public static void DrawCircle(this RenderContext context, ID2DPen pen, PointF center, float radius) {
            context.RenderTarget.Direct2DRenderTarget.DrawCircle(pen, center, radius);
        }

        public static void DrawText(this RenderContext context, string text, ID2DBrush brush, D2DFont font, float destX, float destY) {
            context.RenderTarget.Direct2DRenderTarget.DrawText(text, brush, font, destX, destY);
        }

        public static void Translate(this RenderContext context, float x, float y) {
            context.RenderTarget.Direct2DRenderTarget.Translate(x, y);
        }

        public static void Rotate(this RenderContext context, float angle) {
            context.RenderTarget.Direct2DRenderTarget.Rotate(angle);
        }

        public static void ResetTransform(this RenderContext context) {
            context.RenderTarget.Direct2DRenderTarget.ResetTransform();
        }

        public static void PushTransform(this RenderContext context) {
            context.RenderTarget.PushTransform();
        }

        public static Matrix3x2 PopTransform(this RenderContext context) {
            return context.RenderTarget.PopTransform();
        }

    }
}
