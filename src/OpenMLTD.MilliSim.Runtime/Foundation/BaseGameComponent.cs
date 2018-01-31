using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Configuration;

namespace OpenMLTD.MilliSim.Foundation {
    public abstract class BaseGameComponent : GameComponent, IBaseGameComponent {

        // ReSharper disable once SuggestBaseTypeForParameter
        protected BaseGameComponent([NotNull] BaseGame game, [CanBeNull] IBaseGameComponentContainer parent)
            : base(game) {
            Parent = parent;
            ConfigurationStore = parent?.ConfigurationStore;
        }

        [CanBeNull]
        public IBaseGameComponentContainer Parent {
            get => _parent;
            set {
                _parent?.Components.Remove(this);
                _parent = value;

                if (value != this) {
                    value?.Components.Add(this);
                }
            }
        }

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
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }
                var b = _name != value;
                _name = value;
                _isNameSet = true;

                if (b) {
                    _isNameChanged = true;
                }
            }
        }

        public ConfigurationStore ConfigurationStore { get; internal set; }

        public override string ToString() {
            var typeName = _cachedTypeName ?? (_cachedTypeName = GetType().Name);

            if (_isNameChanged || _cachedStringRepresentation == null) {
                if (!string.IsNullOrEmpty(Name)) {
                    _cachedStringRepresentation = $"{Name} {{{typeName}}}";
                } else {
                    _cachedStringRepresentation = $"{{{typeName}}}";
                }

                _isNameChanged = false;
            }

            return _cachedStringRepresentation;
        }

        public sealed override void Initialize() {
            if (_isInitialized) {
                return;
            }

            _isInitialized = true;

            base.Initialize();
            OnInitialize();
        }

        public sealed override void Update(GameTime gameTime) {
            if (Enabled) {
                OnUpdate(gameTime);
            }
        }

        public bool AreContentsLoaded { get; private set; }

        void IBaseGameComponent.LoadContents() {
            if (AreContentsLoaded) {
                return;
            }

            OnLoadContents();
            AreContentsLoaded = true;
        }

        void IBaseGameComponent.UnloadContents() {
            if (!AreContentsLoaded) {
                return;
            }

            OnUnloadContents();
            AreContentsLoaded = false;
        }

        protected virtual void OnUpdate([NotNull] GameTime gameTime) {
        }

        protected virtual void OnInitialize() {
        }

        protected virtual void OnLoadContents() {
        }

        protected virtual void OnUnloadContents() {
        }

        private bool _isInitialized;
        private bool _isNameSet;
        private bool _isNameChanged;
        private string _cachedStringRepresentation;
        private string _name;
        private string _cachedTypeName;
        private IBaseGameComponentContainer _parent;

    }
}
