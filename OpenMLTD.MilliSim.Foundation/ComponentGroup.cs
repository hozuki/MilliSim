using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Foundation {
    public class ComponentGroup : Component, IComponentContainer {

        public ComponentGroup([NotNull] IComponentContainer parent)
            : base(parent) {
            Components = new ComponentCollection(this);
        }

        public ComponentCollection Components { get; }

        protected override void OnInitialize() {
            base.OnInitialize();
            foreach (var component in Components) {
                component.OnInitialize();
            };
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

        protected virtual void OnBeforeChildrenUpdate([NotNull] GameTime gameTime) {
        }

        protected virtual void OnAfterChildrenUpdate([NotNull] GameTime gameTime) {
        }

        void IComponentContainer.OnBeforeChildrenUpdate(GameTime gameTime) {
            OnBeforeChildrenUpdate(gameTime);
        }

        void IComponentContainer.OnAfterChildrenUpdate(GameTime gameTime) {
            OnAfterChildrenUpdate(gameTime);
        }

    }
}
