using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Foundation {
    public abstract class Element : DisposableBase, IElement {

        protected Element(GameBase game) {
            Game = game;
        }

        public GameBase Game { get; }

        [NotNull]
        public virtual string Name { get; set; } = string.Empty;

        public virtual bool Enabled { get; set; } = true;

        public void Update(GameTime gameTime) {
            if (!Enabled) {
                return;
            }
            OnUpdate(gameTime);
        }

        public override string ToString() {
            if (!string.IsNullOrEmpty(Name)) {
                return Name;
            }
            return base.ToString();
        }

        public void Enable() {
            Enabled = true;
        }

        public void Disable() {
            Enabled = false;
        }

        void IElement.OnInitialize() {
            if (_isInitialized) {
                return;
            }
            OnInitialize();
            _isInitialized = true;
        }

        void IElement.OnDispose() {
            if (!IsDisposed) {
                Dispose();
            }
        }

        protected virtual void OnInitialize() {
        }

        protected virtual void OnUpdate(GameTime gameTime) {
        }

        protected virtual void OnDispose() {
        }

        protected sealed override void Dispose(bool disposing) {
            if (disposing) {
                OnDispose();
            }
        }

        private bool _isInitialized;

    }
}
