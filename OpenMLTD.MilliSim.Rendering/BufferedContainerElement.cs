using System.Collections.Generic;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Rendering {
    public abstract class BufferedContainerElement : Element2D, IContainerElement {

        protected BufferedContainerElement(IReadOnlyList<IElement> elements) {
            Elements = elements;
        }

        public IReadOnlyList<IElement> Elements { get; }

        protected override void OnInitialize() {
            base.OnInitialize();
            foreach (var element in Elements) {
                element.Initialize();
            }
        }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);
            foreach (var element in Elements) {
                element.Update(gameTime);
            }
        }

        protected override void OnDraw(GameTime gameTime, RenderContext context) {
            base.OnDraw(gameTime, context);
            foreach (var element in Elements) {
                (element as IDrawable)?.Draw(gameTime, context);
            }
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);
            foreach (var element in Elements) {
                (element as IDrawable)?.OnGotContext(context);
            }
        }

        protected override void OnLostContext(RenderContext context) {
            base.OnLostContext(context);
            foreach (var element in Elements) {
                (element as IDrawable)?.OnLostContext(context);
            }
        }

    }
}
