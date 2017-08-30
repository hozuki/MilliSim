using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Rendering {
    // Hmm... no IDisposable.
    public abstract class Element : IElement, IUpdateable {

        [NotNull]
        public virtual string Name { get; set; } = string.Empty;

        public virtual bool Enabled { get; set; } = true;

        public void Initialize() {
            if (_isInitialized) {
                return;
            }
            OnInitialize();
            _isInitialized = true;
        }

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

        protected virtual void OnInitialize() {
        }

        protected virtual void OnUpdate(GameTime gameTime) {
        }

        private bool _isInitialized;

    }
}
