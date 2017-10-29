using System;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Foundation {
    public abstract class Component : DisposableBase, IComponent {

        protected Component([NotNull] IComponentContainer parent) {
            Parent = parent;
            Game = parent != null ? parent.Game : null;
        }

        [CanBeNull]
        public IComponentContainer Parent {
            get => _parent;
            set {
                _parent?.Components.Remove(this);
                _parent = value;
                value?.Components.Add(this);
            }
        }

        [NotNull]
        public GameBase Game { get; internal set; }

        [NotNull]
        public virtual string Name {
            get {
                if (_isNameSet) {
                    return _name;
                } else {
                    return _cachedTypeName ?? (_cachedTypeName = GetType().Name);
                }
            }
            set {
                _name = value ?? throw new ArgumentNullException(nameof(value));
                _isNameSet = true;
            }
        }

        public virtual bool Enabled { get; set; } = true;

        public void Update(GameTime gameTime) {
            if (!Enabled) {
                return;
            }
            OnUpdate(gameTime);
        }

        public override string ToString() {
            var typeName = _cachedTypeName ?? (_cachedTypeName = GetType().Name);
            if (!string.IsNullOrEmpty(Name)) {
                return $"{Name} {{{typeName}}}";
            }
            return $"{{{typeName}}}";
        }

        void IComponent.OnInitialize() {
            if (_isInitialized) {
                return;
            }
            OnInitialize();
            _isInitialized = true;
        }

        void IComponent.OnDispose() {
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
        private bool _isNameSet;
        private string _name;
        private string _cachedTypeName;
        private IComponentContainer _parent;

    }
}
