using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Graphics.Drawing {
    public abstract class BufferedVisualContainer : BufferedVisual2D, IVisualContainer {

        protected BufferedVisualContainer([NotNull] IVisualContainer parent)
            : base(parent) {
            Components = new ComponentCollection(this);
        }

        public ComponentCollection Components { get; }

        protected override void OnInitialize() {
            base.OnInitialize();
            foreach (var component in Components) {
                component.OnInitialize();
            }
        }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);
            if (Components.HasPendingEntries) {
                Components.ExecutePendingQueue();
            }
            OnBeforeChildrenUpdate(gameTime);
            foreach (var component in Components) {
                component.Update(gameTime);
            }
            OnAfterChildrenUpdate(gameTime);
        }

        protected override void OnDrawBuffer(GameTime gameTime, RenderContext context) {
            base.OnDrawBuffer(gameTime, context);
            OnBeforeChildrenDraw(gameTime, context);
            foreach (var component1 in Components) {
                (component1 as IDrawable)?.Draw(gameTime, context);
            }
            OnAfterChildrenDraw(gameTime, context);
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);
            foreach (var component in Components) {
                (component as IDrawable)?.OnGotContext(context);
            }
        }

        protected override void OnLostContext(RenderContext context) {
            base.OnLostContext(context);
            foreach (var component in Components) {
                (component as IDrawable)?.OnLostContext(context);
            }
        }

        protected override void OnStageReady(RenderContext context) {
            base.OnStageReady(context);
            foreach (var component in Components) {
                (component as IDrawable)?.OnStageReady(context);
            }
        }

        protected override void OnLayout() {
            base.OnLayout();
            foreach (var component in Components) {
                (component as IDrawable)?.OnLayout();
            }
        }

        protected override void OnDispose() {
            foreach (var component in Components) {
                component.OnDispose();
            }
            base.OnDispose();
        }

        protected void OnBeforeChildrenUpdate([NotNull] GameTime gameTime) {
        }

        protected void OnAfterChildrenUpdate([NotNull] GameTime gameTime) {
        }

        protected void OnBeforeChildrenDraw([NotNull] GameTime gameTime, [NotNull] RenderContext context) {
        }

        protected void OnAfterChildrenDraw([NotNull] GameTime gameTime, [NotNull] RenderContext context) {
        }

        void IComponentContainer.OnBeforeChildrenUpdate(GameTime gameTime) {
            OnBeforeChildrenUpdate(gameTime);
        }

        void IComponentContainer.OnAfterChildrenUpdate(GameTime gameTime) {
            OnAfterChildrenUpdate(gameTime);
        }

        void IVisualContainer.OnBeforeChildrenDraw(GameTime gameTime, RenderContext context) {
            OnBeforeChildrenDraw(gameTime, context);
        }

        void IVisualContainer.OnAfterChildrenDraw(GameTime gameTime, RenderContext context) {
            OnAfterChildrenDraw(gameTime, context);
        }

    }
}
