using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Graphics.Rendering;
using OpenMLTD.MilliSim.Graphics.Rendering.Direct3D;
using OpenMLTD.MilliSim.Graphics.Rendering.Direct3D.Effects;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Color = System.Drawing.Color;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public class RibbonsLayer : VisualElement {

        public RibbonsLayer(GameBase game)
            : base(game) {
        }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);
            _camera.UpdateViewMatrix();
        }

        protected override void OnDraw(GameTime gameTime, RenderContext context) {
            base.OnDraw(gameTime, context);

            var viewProjection = _camera.ViewProjectionMatrix;

            context.Begin3D();
            var ctx = context.RenderTarget.DeviceContext3D;
            ctx.Rasterizer.State = _noCullRasState;
            ctx.OutputMerger.BlendState = _alphaBlendState;
            ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            ctx.InputAssembler.InputLayout = _posTexLayout;
            _modelInstance.Draw(context.RenderTarget.DeviceContext3D, _textureEffect, viewProjection);
            //_modelInstance.Draw(context.RenderTarget.DeviceContext3D, _colorEffect, viewProjection);
            ctx.InputAssembler.InputLayout = null;
            ctx.Rasterizer.State = null;
            ctx.OutputMerger.BlendState = null;
            context.End3D();
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);

            _camera = new OrthoCamera(context.ClientSize.Width, context.ClientSize.Height, -1000f, 1000f);
            _camera.Position = new Vector3(50, 50, 50);
            _camera.LookAtZUp(Vector3.Zero);

            var settings = Program.Settings;

            _ribbonTexture = Direct3DHelper.LoadTexture2D(context, settings.Images.Ribbon.FileName);
            _ribbonTextureSrv = _ribbonTexture.CreateResourceView();

            var textureEffect = context.CreateD3DEffectFromFile<D3DSimpleTextureEffect>("res/fx/simple_texture.fx");
            _textureEffect = textureEffect;
            textureEffect.WorldTransform = Matrix.Identity;
            textureEffect.Texture = _ribbonTextureSrv;
            textureEffect.TextureTransform = Matrix.Identity;

            var colorEffect = context.CreateD3DEffectFromFile<D3DVertexColorEffect>("res/fx/vertex_color.fx");
            _colorEffect = colorEffect;

            var pass = textureEffect.SimpleTextureTechnique.GetPassByIndex(0);
            var passDesc = pass.Description;
            var bytecode = passDesc.Signature;
            _posTexLayout = new InputLayout(context.Direct3DDevice, bytecode, InputLayoutDescriptions.PosNormTex);

            _model = BasicModel.CreateCylinder(context.Direct3DDevice, 60, 60, 60, 100, 200);
            _model.DiffuseMap = _ribbonTextureSrv;
            _model.Material = new D3DMaterial {
                Diffuse = Color.White.ToC4()
            };
            _modelInstance = new BasicModelInstance(_model);

            var noCullDesc = new RasterizerStateDescription {
                FillMode = FillMode.Solid,
                CullMode = CullMode.None,
                IsFrontCounterClockwise = false,
                IsDepthClipEnabled = true
            };
            _noCullRasState = new RasterizerState(context.Direct3DDevice, noCullDesc);

            var transDesc = new BlendStateDescription {
                AlphaToCoverageEnable = false,
                IndependentBlendEnable = false
            };
            transDesc.RenderTarget[0].IsBlendEnabled = true;
            transDesc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
            transDesc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            transDesc.RenderTarget[0].BlendOperation = BlendOperation.Add;
            transDesc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
            transDesc.RenderTarget[0].DestinationAlphaBlend = BlendOption.Zero;
            transDesc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
            transDesc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;

            _alphaBlendState = new BlendState(context.Direct3DDevice, transDesc);
        }

        protected override void OnLostContext(RenderContext context) {
            base.OnLostContext(context);

            _model.Dispose();
            _posTexLayout.Dispose();
            _textureEffect.Dispose();
            _colorEffect.Dispose();
            _ribbonTextureSrv.Dispose();
            _ribbonTexture.Dispose();

            _noCullRasState.Dispose();
            _alphaBlendState.Dispose();
        }

        private OrthoCamera _camera;
        private BasicModel _model;
        private BasicModelInstance _modelInstance;
        private Texture2D _ribbonTexture;
        private ShaderResourceView _ribbonTextureSrv;
        private InputLayout _posTexLayout;
        private D3DSimpleTextureEffect _textureEffect;
        private D3DVertexColorEffect _colorEffect;
        private RasterizerState _noCullRasState;
        private BlendState _alphaBlendState;

    }
}
