using System.Drawing;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Graphics {
    public abstract class Visual : Component, IVisual {

        protected Visual([NotNull] IVisualContainer parent)
            : base(parent) {
        }

        public void Draw(GameTime gameTime, RenderContext context) {
            if (!Visible) {
                return;
            }
            OnDraw(gameTime, context);
        }

        public virtual bool Visible { get; set; } = true;

        public void PerformLayout() {
            var clientSize = Game.Window.ClientSize;
            OnLayout(clientSize);
        }

        protected virtual void OnDraw([NotNull] GameTime gameTime, [NotNull] RenderContext context) {
        }

        protected virtual void OnGotContext([NotNull] RenderContext context) {
        }

        protected virtual void OnLostContext([NotNull] RenderContext context) {
        }

        protected virtual void OnStageReady([NotNull] RenderContext context) {
        }

        protected virtual void OnLayout(Size clientSize) {
        }

        void IDrawable.OnGotContext(RenderContext context) {
            OnGotContext(context);
        }

        void IDrawable.OnLostContext(RenderContext context) {
            OnLostContext(context);
        }

        void IDrawable.OnStageReady(RenderContext context) {
            OnStageReady(context);
        }

        void IDrawable.OnLayout(Size clientSize) {
            OnLayout(clientSize);
        }

    }
}
