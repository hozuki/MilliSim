using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Graphics {
    /// <inheritdoc cref="Visual"/>
    /// <inheritdoc cref="IVisualContainer"/>
    /// <summary>
    /// A basic implementation for <see cref="IVisualContainer" />.
    /// </summary>
    public class VisualContainer : Visual, IVisualContainer {

        /// <inheritdoc />
        /// <summary>
        /// Creates a new <see cref="T:OpenMLTD.MilliSim.Graphics.VisualContainer" />.
        /// </summary>
        /// <param name="game">The base game.</param>
        /// <param name="parent">The parent of this <see cref="T:OpenMLTD.MilliSim.Graphics.VisualContainer" />.</param>
        public VisualContainer([NotNull] BaseGame game, [CanBeNull] IVisualContainer parent)
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

        void IVisualContainer.OnBeforeChildrenDraw(GameTime gameTime) {
            OnBeforeChildrenDraw(gameTime);
        }

        void IVisualContainer.OnAfterChildrenDraw(GameTime gameTime) {
            OnAfterChildrenDraw(gameTime);
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

        protected override void OnDraw(GameTime gameTime) {
            base.OnDraw(gameTime);

            OnSelfDraw(gameTime);

            OnBeforeChildrenDraw(gameTime);

            foreach (var component in Components) {
                if (component is IDrawable drawable) {
                    drawable.Draw(gameTime);
                }
            }

            OnAfterChildrenDraw(gameTime);
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

        /// <summary>
        /// Called when this object is being updated.
        /// </summary>
        /// <param name="gameTime">Current time.</param>
        protected virtual void OnSelfUpdate([NotNull] GameTime gameTime) {
        }

        /// <summary>
        /// Called before updating children.
        /// </summary>
        /// <param name="gameTime">Current time.</param>
        protected virtual void OnBeforeChildrenUpdate([NotNull] GameTime gameTime) {
        }

        /// <summary>
        /// Called after updating children.
        /// </summary>
        /// <param name="gameTime">Current time.</param>
        protected virtual void OnAfterChildrenUpdate([NotNull] GameTime gameTime) {
        }

        /// <summary>
        /// Called when this object is being drawn.
        /// </summary>
        /// <param name="gameTime">Current time.</param>
        protected virtual void OnSelfDraw([NotNull] GameTime gameTime) {
        }

        /// <summary>
        /// Called before drawing children.
        /// </summary>
        /// <param name="gameTime">Current time.</param>
        protected virtual void OnBeforeChildrenDraw([NotNull] GameTime gameTime) {
        }

        /// <summary>
        /// Called after drawing children.
        /// </summary>
        /// <param name="gameTime">Current time.</param>
        protected virtual void OnAfterChildrenDraw([NotNull] GameTime gameTime) {
        }

    }
}
