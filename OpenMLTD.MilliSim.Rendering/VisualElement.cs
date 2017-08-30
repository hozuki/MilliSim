using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Rendering {
    public abstract class VisualElement : Element, IDrawable {

        public void Draw(GameTime gameTime, RenderContext context) {
            if (!Visible) {
                return;
            }
            OnDraw(gameTime, context);
        }

        public virtual bool Visible { get; set; } = true;

        public void Show() {
            Visible = true;
        }

        public void Hide() {
            Visible = false;
        }

        protected virtual void OnDraw(GameTime gameTime, RenderContext context) {
        }

        protected virtual void OnGotContext(RenderContext context) {
        }

        protected virtual void OnLostContext(RenderContext context) {
        }

        void IDrawable.OnGotContext(RenderContext context) {
            OnGotContext(context);
        }

        void IDrawable.OnLostContext(RenderContext context) {
            OnLostContext(context);
        }

    }
}
