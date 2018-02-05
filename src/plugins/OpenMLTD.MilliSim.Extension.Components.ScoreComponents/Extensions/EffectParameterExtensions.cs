using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Extensions {
    internal static class EffectParameterExtensions {

        // ReSharper disable once InconsistentNaming
        internal static Matrix3x2 GetValueMatrix3x2([NotNull] this EffectParameter parameter) {
            var data = parameter.GetValueVector3Array();

            return new Matrix3x2(data[0].X, data[0].Y, data[1].X, data[1].Y, data[2].X, data[2].Y);
        }

        internal static void SetValue([NotNull] this EffectParameter parameter, Matrix3x2 value) {
            var data = new[] {
                new Vector3(value.M11, value.M12, 0),
                new Vector3(value.M21, value.M22, 0),
                new Vector3(value.M31, value.M32, 1)
            };

            parameter.SetValue(data);
        }

    }
}
