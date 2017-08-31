using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Rendering {
    public abstract class VisualGame : GameBase {

        protected VisualGame([CanBeNull, ItemNotNull] IReadOnlyList<Element> elements)
            : base(elements) {
            var visualElements = Elements.OfType<VisualElement>().ToArray();
            Stage = new StageView(visualElements);
        }

        public override void Draw(GameTime gameTime) {
            Stage.Draw(gameTime, StageRenderer);
        }

        public ControlStageRenderer StageRenderer => (ControlStageRenderer)Renderer;

        public StageView Stage { get; }

        protected override RendererBase CreateRenderer() {
            return new ControlStageRenderer(this, Window);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                var renderContext = StageRenderer.RenderContext;
                foreach (var element in Elements) {
                    (element as IDrawable)?.OnLostContext(renderContext);
                }
                Renderer.Dispose();
            }

            base.Dispose(disposing);
        }

    }
}
