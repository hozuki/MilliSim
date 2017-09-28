using System;
using System.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D.Advanced;
using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    partial class RenderContextExtensions {

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

        public static void DrawImageStripUnit(this RenderContext context, D2DImageStrip imageStrip, int index, float destX, float destY) {
            if (index < 0 || index >= imageStrip.Count) {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            var srcRect = GetSourceRect(imageStrip, index);
            context.DrawImage(imageStrip, destX, destY, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height);
        }

        public static void DrawImageStripUnit(this RenderContext context, D2DImageStrip imageStrip, int index, float destX, float destY, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            if (index < 0 || index >= imageStrip.Count) {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            var srcRect = GetSourceRect(imageStrip, index);
            context.DrawImage(imageStrip, destX, destY, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, interpolationMode, compositeMode);
        }

        public static void DrawImageStripUnit(this RenderContext context, D2DImageStrip imageStrip, int index, float destX, float destY, float destWidth, float destHeight) {
            if (index < 0 || index >= imageStrip.Count) {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            var srcRect = GetSourceRect(imageStrip, index);
            context.DrawBitmap(imageStrip, destX, destY, destWidth, destHeight, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height);
        }

        public static void DrawImageStripUnit(this RenderContext context, D2DImageStrip imageStrip, int index, float destX, float destY, float destFullWidth, float destFullHeight, float blankLeft, float blankTop, float blankRight, float blankBottom) {
            if (index < 0 || index >= imageStrip.Count) {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            var blankEdge = RectangleF.FromLTRB(blankLeft, blankTop, blankRight, blankBottom);
            var srcRect = GetSourceRect(imageStrip, index, blankEdge);
            context.DrawBitmap(imageStrip, destX, destY, destFullWidth, destFullHeight, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height);
        }

        public static void DrawImageStripUnit(this RenderContext context, D2DImageStrip imageStrip, int index, float destX, float destY, float destWidth, float destHeight, float opacity) {
            if (index < 0 || index >= imageStrip.Count) {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            var srcRect = GetSourceRect(imageStrip, index);
            context.DrawBitmap(imageStrip, destX, destY, destWidth, destHeight, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, opacity);
        }

        public static void DrawImageStripUnit(this RenderContext context, D2DImageStrip2D imageStrip, int index, float destX, float destY) {
            if (index < 0 || index >= imageStrip.Count) {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            var srcRect = GetSourceRect(imageStrip, index);
            context.DrawImage(imageStrip, destX, destY, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height);
        }

        public static void DrawImageStripUnit(this RenderContext context, D2DImageStrip2D imageStrip, int index, float destX, float destY, InterpolationMode interpolationMode, CompositeMode compositeMode) {
            if (index < 0 || index >= imageStrip.Count) {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            var srcRect = GetSourceRect(imageStrip, index);
            context.DrawImage(imageStrip, destX, destY, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, interpolationMode, compositeMode);
        }

        public static void DrawImageStripUnit(this RenderContext context, D2DImageStrip2D imageStrip, int index, float destX, float destY, float destWidth, float destHeight) {
            if (index < 0 || index >= imageStrip.Count) {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            var srcRect = GetSourceRect(imageStrip, index);
            context.DrawBitmap(imageStrip, destX, destY, destWidth, destHeight, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height);
        }

        public static void DrawImageStripUnit(this RenderContext context, D2DImageStrip2D imageStrip, int index, float destX, float destY, float destWidth, float destHeight, float opacity) {
            if (index < 0 || index >= imageStrip.Count) {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            var srcRect = GetSourceRect(imageStrip, index);
            context.DrawBitmap(imageStrip, destX, destY, destWidth, destHeight, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, opacity);
        }

        private static RectangleF GetSourceRect(D2DImageStrip imageStrip, int index, RectangleF blankEdge) {
            var rect = GetSourceRect(imageStrip, index);
            return new RectangleF(rect.Left + blankEdge.Left, rect.Top + blankEdge.Top, rect.Width - (blankEdge.Left + blankEdge.Right), rect.Height - (blankEdge.Top + blankEdge.Bottom));
        }

        private static RectangleF GetSourceRect(D2DImageStrip imageStrip, int index) {
            if (index < 0 || index >= imageStrip.Count) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            float x, y, w, h;
            switch (imageStrip.Orientation) {
                case ImageStripOrientation.Horizontal:
                    w = imageStrip.UnitWidth;
                    h = imageStrip.UnitHeight;
                    x = index * w;
                    y = 0;
                    break;
                case ImageStripOrientation.Vertical:
                    w = imageStrip.UnitWidth;
                    h = imageStrip.UnitHeight;
                    x = 0;
                    y = index * h;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new RectangleF(x, y, w, h);
        }

        private static RectangleF GetSourceRect(D2DImageStrip2D imageStrip, int index) {
            if (index < 0 || index >= imageStrip.Count) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            int xIndex, yIndex;
            float w, h;
            switch (imageStrip.Orientation) {
                case ImageStripOrientation.Horizontal:
                    w = imageStrip.UnitWidth;
                    h = imageStrip.UnitHeight;
                    xIndex = index % imageStrip.ArrayCount;
                    yIndex = index / imageStrip.ArrayCount;
                    break;
                case ImageStripOrientation.Vertical:
                    w = imageStrip.UnitWidth;
                    h = imageStrip.UnitHeight;
                    xIndex = index / imageStrip.ArrayCount;
                    yIndex = index % imageStrip.ArrayCount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var x = xIndex * w;
            var y = yIndex * h;

            return new RectangleF(x, y, w, h);
        }

    }
}
