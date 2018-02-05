using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace OpenMLTD.MilliSim.Foundation {
    public class BaseGameComponentContainer : BaseGameComponent, IBaseGameComponentContainer {

        public BaseGameComponentContainer([NotNull] BaseGame game, [NotNull] IBaseGameComponentContainer parent)
            : base(game, parent) {
            Components = new BaseGameComponentCollection(this);
        }

        public BaseGameComponentCollection Components { get; }

        void IBaseGameComponentContainer.OnBeforeChildrenUpdate(GameTime gameTime) {
            OnBeforeChildrenUpdate(gameTime);
        }

        void IBaseGameComponentContainer.OnAfterChildrenUpdate(GameTime gameTime) {
            OnAfterChildrenUpdate(gameTime);
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            foreach (var component in Components) {
                component.Initialize();
            }
        }

        protected override void OnLoadContents() {
            base.OnLoadContents();

            foreach (var component in Components) {
                component.LoadContents();
            }
        }

        protected override void OnUnloadContents() {
            foreach (var component in Components) {
                component.UnloadContents();
            }

            base.OnUnloadContents();
        }

        protected sealed override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            OnSelfUpdate(gameTime);

            OnBeforeChildrenUpdate(gameTime);

            foreach (var component in Components) {
                component.Update(gameTime);
            }

            OnAfterChildrenUpdate(gameTime);

            Components.ExecutePendingQueue();
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                foreach (var component in Components) {
                    component.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        protected virtual void OnSelfUpdate([NotNull] GameTime gameTime) {
        }

        protected virtual void OnBeforeChildrenUpdate([NotNull] GameTime gameTime) {
        }

        protected virtual void OnAfterChildrenUpdate([NotNull] GameTime gameTime) {
        }

    }
}
