using SharpDX;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    public static class Matrix3x2Extensions {

        public static Matrix3x2 AppendTranslation(this Matrix3x2 matrix, float x, float y) {
            var m = Matrix3x2.Translation(x, y);
            return matrix * m;
        }

        public static Matrix3x2 AppendTranslation(this Matrix3x2 matrix, Vector2 offset) {
            return AppendTranslation(matrix, offset.X, offset.Y);
        }

        public static Matrix3x2 PrependTranslation(this Matrix3x2 matrix, float x, float y) {
            var m = Matrix3x2.Translation(x, y);
            return matrix * m;
        }

        public static Matrix3x2 PrependTranslation(this Matrix3x2 matrix, Vector2 offset) {
            return PrependTranslation(matrix, offset.X, offset.Y);
        }

        public static Matrix3x2 AppendRotation(this Matrix3x2 matrix, float angle) {
            return AppendRotation(matrix, angle, Vector2.Zero);
        }

        public static Matrix3x2 AppendRotation(this Matrix3x2 matrix, float angle, Vector2 center) {
            var m = Matrix3x2.Rotation(angle, center);
            return matrix * m;
        }

        public static Matrix3x2 PrependRotation(this Matrix3x2 matrix, float angle) {
            return PrependRotation(matrix, angle, Vector2.Zero);
        }

        public static Matrix3x2 PrependRotation(this Matrix3x2 matrix, float angle, Vector2 center) {
            var m = Matrix3x2.Rotation(angle, center);
            return m * matrix;
        }

        public static Matrix3x2 AppendScaling(this Matrix3x2 matrix, float x, float y) {
            return AppendScaling(matrix, x, y, Vector2.Zero);
        }

        public static Matrix3x2 AppendScaling(this Matrix3x2 matrix, float x, float y, Vector2 center) {
            var m = Matrix3x2.Scaling(x, y, center);
            return matrix * m;
        }

        public static Matrix3x2 PrependScaling(this Matrix3x2 matrix, float x, float y) {
            return PrependScaling(matrix, x, y, Vector2.Zero);
        }

        public static Matrix3x2 PrependScaling(this Matrix3x2 matrix, float x, float y, Vector2 center) {
            var m = Matrix3x2.Scaling(x, y, center);
            return m * matrix;
        }

        public static Matrix3x2 AppendSkew(this Matrix3x2 matrix, float x, float y) {
            var m = Matrix3x2.Skew(x, y);
            return matrix * m;
        }

        public static Matrix3x2 PrependSkew(this Matrix3x2 matrix, float x, float y) {
            var m = Matrix3x2.Skew(x, y);
            return m * matrix;
        }

        public static Matrix3x2 Negate(this Matrix3x2 matrix) {
            return Matrix3x2.Negate(matrix);
        }

        public static Matrix3x2 Invert(this Matrix3x2 matrix) {
            return Matrix3x2.Invert(matrix);
        }

        public static Matrix3x2 LerpTo(this Matrix3x2 matrix, Matrix3x2 target, float percent) {
            return Matrix3x2.Lerp(matrix, target, percent);
        }

        public static Matrix3x2 SmoothStepTo(this Matrix3x2 matrix, Matrix3x2 target, float percent) {
            return Matrix3x2.SmoothStep(matrix, target, percent);
        }

    }
}
