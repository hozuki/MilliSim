using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Rendering {
    public abstract class Game : GameBase {

        protected Game([CanBeNull, ItemNotNull] IReadOnlyList<Element> elements)
            : base(elements) {
            var visualElements = Elements.OfType<VisualElement>().ToArray();
            Stage = new StageView(visualElements);
        }

        public override void Draw(GameTime gameTime) {
            Stage.Draw(gameTime, Renderer);
        }

        public ControlStageRenderer Renderer { get; private set; }

        public StageView Stage { get; }

        protected override void OnBeforeRun(EventArgs e) {
            base.OnBeforeRun(e);
            Renderer = new ControlStageRenderer(this, Window);
        }

        protected override void OnAfterRun(EventArgs e) {
            Renderer.Dispose();
            base.OnAfterRun(e);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                var renderContext = Renderer.RenderContext;
                foreach (var element in Elements) {
                    (element as IDrawable)?.OnLostContext(renderContext);
                }
                Renderer.Dispose();
            }

            base.Dispose(disposing);
        }

    }
}
