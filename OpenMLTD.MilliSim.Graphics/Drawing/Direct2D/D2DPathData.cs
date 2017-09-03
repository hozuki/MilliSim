using System;
using System.Drawing;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Graphics.Extensions;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace OpenMLTD.MilliSim.Graphics.Drawing.Direct2D {
    public sealed class D2DPathData : DisposableBase {

        public D2DPathData(Factory factory) {
            _geometry = new PathGeometry(factory);
        }

        public void BeginDraw() {
            if (_sink != null) {
                return;
            }
            _sink = _geometry.Open();
        }

        public void EndDraw() {
            if (_sink == null) {
                return;
            }
            _sink.Close();
            _sink = null;
        }

        public void BeginFigure(PointF point) {
            BeginFigure(point.X, point.Y);
        }

        public void BeginFigure(Point point) {
            BeginFigure(point.X, point.Y);
        }

        public void BeginFigure(float x, float y) {
            EnsureSinkNotNull();
            _sink.BeginFigure(new RawVector2(x, y), FigureBegin.Filled);
        }

        public void BeginFigure(PointF point, bool filled) {
            BeginFigure(point.X, point.Y, filled);
        }

        public void BeginFigure(Point point, bool filled) {
            BeginFigure(point.X, point.Y, filled);
        }

        public void BeginFigure(float x, float y, bool filled) {
            EnsureSinkNotNull();
            _sink.BeginFigure(new RawVector2(x, y), filled ? FigureBegin.Filled : FigureBegin.Hollow);
        }

        public void EndFigure() {
            EndFigure(true);
        }

        public void EndFigure(bool closed) {
            EnsureSinkNotNull();
            _sink.EndFigure(closed ? FigureEnd.Closed : FigureEnd.Open);
        }

        /// <summary>
        /// This method can only be called after a sink is opened.
        /// </summary>
        /// <param name="fillMode"></param>
        public void SetFillMode(FillMode fillMode) {
            EnsureSinkNotNull();
            _sink.SetFillMode((SharpDX.Direct2D1.FillMode)fillMode);
        }

        public void AddArc(D2DArcSegment arc) {
            EnsureSinkNotNull();
            _sink.AddArc(arc.ToNative());
        }

        public void AddBezier(Point control1, Point control2, Point end) {
            EnsureSinkNotNull();
            var bezier = new BezierSegment {
                Point1 = control1.ToD2DVector(),
                Point2 = control2.ToD2DVector(),
                Point3 = end.ToD2DVector()
            };
            _sink.AddBezier(bezier);
        }

        public void AddBezier(PointF control1, PointF control2, PointF end) {
            EnsureSinkNotNull();
            var bezier = new BezierSegment {
                Point1 = control1.ToD2DVector(),
                Point2 = control2.ToD2DVector(),
                Point3 = end.ToD2DVector()
            };
            _sink.AddBezier(bezier);
        }

        public void AddBezier(float cx1, float cy1, float cx2, float cy2, float x, float y) {
            EnsureSinkNotNull();
            var bezier = new BezierSegment {
                Point1 = new RawVector2(cx1, cy1),
                Point2 = new RawVector2(cx2, cy2),
                Point3 = new RawVector2(x, y)
            };
            _sink.AddBezier(bezier);
        }

        public void AddBezier(D2DBezierSegment bezier) {
            EnsureSinkNotNull();
            _sink.AddBezier(bezier.ToNative());
        }

        public void AddBeziers(params D2DBezierSegment[] beziers) {
            EnsureSinkNotNull();
            var b = new BezierSegment[beziers.Length];
            for (var i = 0; i < beziers.Length; ++i) {
                b[i] = beziers[i].ToNative();
            }
            _sink.AddBeziers(b);
        }

        public void AddLine(Point point) {
            EnsureSinkNotNull();
            _sink.AddLine(point.ToD2DVector());
        }

        public void AddLine(PointF point) {
            EnsureSinkNotNull();
            _sink.AddLine(point.ToD2DVector());
        }

        public void AddLine(float x, float y) {
            EnsureSinkNotNull();
            _sink.AddLine(new RawVector2(x, y));
        }

        public void AddLines(params PointF[] points) {
            EnsureSinkNotNull();
            var pts = new RawVector2[points.Length];
            for (var i = 0; i < points.Length; ++i) {
                pts[i] = points[i].ToD2DVector();
            }
            _sink.AddLines(pts);
        }

        public void AddLines(params Point[] points) {
            EnsureSinkNotNull();
            var pts = new RawVector2[points.Length];
            for (var i = 0; i < points.Length; ++i) {
                pts[i] = points[i].ToD2DVector();
            }
            _sink.AddLines(pts);
        }

        public void AddQuadraticBezier(Point control, Point end) {
            EnsureSinkNotNull();
            var bezier = new QuadraticBezierSegment {
                Point1 = control.ToD2DVector(),
                Point2 = end.ToD2DVector()
            };
            _sink.AddQuadraticBezier(bezier);
        }

        public void AddQuadraticBezier(PointF control, PointF end) {
            EnsureSinkNotNull();
            var bezier = new QuadraticBezierSegment {
                Point1 = control.ToD2DVector(),
                Point2 = end.ToD2DVector()
            };
            _sink.AddQuadraticBezier(bezier);
        }

        public void AddQuadraticBezier(float cx, float cy, float x, float y) {
            EnsureSinkNotNull();
            var bezier = new QuadraticBezierSegment {
                Point1 = new RawVector2(cx, cy),
                Point2 = new RawVector2(x, y)
            };
            _sink.AddQuadraticBezier(bezier);
        }

        public void AddQuadraticBezier(D2DQuadraticBezierSegment bezier) {
            EnsureSinkNotNull();
            _sink.AddQuadraticBezier(bezier.ToNative());
        }

        public void AddQuadraticBeziers(params D2DQuadraticBezierSegment[] beziers) {
            EnsureSinkNotNull();
            var b = new QuadraticBezierSegment[beziers.Length];
            for (var i = 0; i < beziers.Length; ++i) {
                b[i] = beziers[i].ToNative();
            }
            _sink.AddQuadraticBeziers(b);
        }

        internal PathGeometry NativeGeometry => _geometry;

        internal GeometrySink NativeSink => _sink;

        protected override void Dispose(bool disposing) {
            if (disposing) {
                _geometry?.Dispose();
                _geometry = null;
            }
        }

        private void EnsureSinkNotNull() {
            if (_sink == null) {
                throw new NullReferenceException("The geometry sink is null. Call BeginDraw() to open the path and create the sink.");
            }
        }

        private PathGeometry _geometry;
        private GeometrySink _sink;

    }
}
