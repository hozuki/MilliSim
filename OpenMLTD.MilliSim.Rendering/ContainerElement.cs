using System.Collections.Generic;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Rendering {
    public abstract class ContainerElement : Element2D {

        protected ContainerElement(GameBase game, [CanBeNull, ItemNotNull] IReadOnlyList<IElement> elements)
            : base(game) {
            Elements = elements ?? new Element[0];
        }

        public IReadOnlyList<IElement> Elements { get; }

        protected override void OnInitialize() {
            base.OnInitialize();
            foreach (var element in Elements) {
                element.OnInitialize();
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

        protected override void OnDispose() {
            foreach (var element in Elements) {
                element.OnDispose();
            }
            base.OnDispose();
        }

    }
}
