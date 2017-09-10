using System;
using System.Reflection;
using OpenMLTD.MilliSim.Graphics.Rendering;
using OpenMLTD.MilliSim.Graphics.Rendering.Direct3D;
using OpenMLTD.MilliSim.Graphics.Rendering.Direct3D.Effects;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    partial class RenderContextExtensions {

        public static InputLayout CreateInputLayout(this RenderContext context, EffectPass pass, params InputElement[] elements) {
            return CreateInputLayout(context, pass.Description.Signature, elements);
        }

        public static InputLayout CreateInputLayout(this RenderContext context, ShaderBytecode bytecode, params InputElement[] elements) {
            return new InputLayout(context.Direct3DDevice, bytecode, elements);
        }

        public static T CreateD3DEffect<T>(this RenderContext context, string shaderSource) where T : D3DEffect {
            var t = typeof(T);
            var ctor = t.GetConstructor(D3DEffectConstructorBindingFlags, null, D3DEffectConstructorSignature, null);
            var effect = (T)ctor.Invoke(new object[] { context.Direct3DDevice, shaderSource, true });
            effect.Compile();
            effect.Initialize();
            return effect;
        }

        public static T CreateD3DEffectFromFile<T>(this RenderContext context, string fileName) where T : D3DEffect {
            var t = typeof(T);
            var ctor = t.GetConstructor(D3DEffectConstructorBindingFlags, null, D3DEffectConstructorSignature, null);
            var effect = (T)ctor.Invoke(new object[] { context.Direct3DDevice, fileName, false });
            effect.Compile();
            effect.Initialize();
            return effect;
        }

        public static void DrawModel(this RenderContext context, BasicModelInstance modelInstance, D3DSimpleTextureEffect effect, Matrix viewProjection) {
            modelInstance.Draw(context.Direct3DDevice.ImmediateContext, effect, viewProjection);
        }

        public static void DrawModel(this RenderContext context, BasicModelInstance modelInstance, D3DVertexColorEffect effect, Matrix viewProjection) {
            modelInstance.Draw(context.Direct3DDevice.ImmediateContext, effect, viewProjection);
        }

        private const BindingFlags D3DEffectConstructorBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        private static readonly Type[] D3DEffectConstructorSignature = { typeof(Device), typeof(string), typeof(bool) };

    }
}
