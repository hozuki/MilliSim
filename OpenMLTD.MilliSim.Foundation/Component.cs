using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Configuration;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Extensions;

namespace OpenMLTD.MilliSim.Foundation {
    public abstract class Component : DisposableBase, IComponent {

        protected Component([NotNull] IComponentContainer parent) {
            Parent = parent;
            Game = parent != null ? parent.Game : null;
            ConfigurationStore = parent != null ? parent.ConfigurationStore : null;
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

        [NotNull]
        public ConfigurationStore ConfigurationStore { get; internal set; }

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

        public static Component CreateAndAddTo([NotNull] IComponentContainer parent, [NotNull] Type type) {
            if (!type.IsSubclassOf(typeof(Component))) {
                throw new ArgumentException($"{type} is not a subclass of {typeof(Component)}");
            }

            var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (ctors.Length > 0) {
                ctors = ctors.Where(c => {
                    var parameters = c.GetParameters();
                    return parameters.Length == 1 &&
                    (parameters[0].ParameterType == typeof(IComponentContainer) || parameters[0].ParameterType.ImplementsInterface(typeof(IComponentContainer)));
                }).ToArray();
            }
            if (ctors.Length == 0) {
                throw new MissingMethodException("Cannot find a proper constructor.");
            }

            var parentType = parent.GetType();
            var ctor = (from c in ctors
                        let param = c.GetParameters()[0]
                        where param.ParameterType == parentType
                        select c).FirstOrDefault();

            if (ctor == null) {
                ctor = ctors[0];
            }

            return (Component)ctor.Invoke(new object[] { parent });
        }

        public static T CreateAndAddTo<T>([NotNull] IComponentContainer parent) where T : Component {
            var t = typeof(T);
            var c = CreateAndAddTo(parent, t);
            return (T)c;
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
