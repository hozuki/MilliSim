using System.Linq;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Graphics {
    public abstract class VisualGame : GameBase {

        public override void Draw(GameTime gameTime) {
            Stage.Draw(gameTime, StageRenderer);
        }

        public ControlStageRenderer StageRenderer => (ControlStageRenderer)BaseRenderer;

        public StageView Stage { get; private set; }

        protected override void OnInitialize() {
            base.OnInitialize();

            var visualElements = Elements.OfType<VisualElement>().ToArray();
            Stage = new StageView(visualElements);
        }

        protected override RendererBase CreateRenderer() {
            return new ControlStageRenderer(this, Window);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                var renderContext = StageRenderer.RenderContext;
                foreach (var element in Elements) {
                    (element as IDrawable)?.OnLostContext(renderContext);
                }
                BaseRenderer.Dispose();
            }

            base.Dispose(disposing);
        }

    }
}
