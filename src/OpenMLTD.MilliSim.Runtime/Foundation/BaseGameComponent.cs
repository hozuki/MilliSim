using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Configuration;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Foundation {
    /// <inheritdoc cref="GameComponent"/>
    /// <inheritdoc cref="IBaseGameComponent"/>
    /// <summary>
    /// Component for <see cref="BaseGame"/>. This class must be inherited.
    /// </summary>
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

        public BaseConfigurationStore ConfigurationStore { get; internal set; }

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

        /// <summary>
        /// Creates a new <see cref="BaseGameComponent"/> and adds it to the <see cref="parent"/>.
        /// The component must have a constructor (public or non-public) of the form <see cref="BaseGameComponent(BaseGame,IBaseGameComponentContainer)"/>.
        /// </summary>
        /// <param name="game">The <see cref="BaseGame"/> instance.</param>
        /// <param name="parent">The parent of created component.</param>
        /// <param name="type">Type of the component.</param>
        /// <returns>Created component.</returns>
        [NotNull]
        public static BaseGameComponent CreateAndAddTo([NotNull] BaseGame game, [NotNull] IBaseGameComponentContainer parent, [NotNull] Type type) {
            if (!type.IsSubclassOf(typeof(BaseGameComponent))) {
                throw new ArgumentException($"{type} is not a subclass of {typeof(BaseGameComponent)}");
            }
            var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (ctors.Length > 0) {
                ctors = ctors.Where(c => {
                    var parameters = c.GetParameters();
                    return parameters.Length == 2 &&
                           (parameters[0].ParameterType == typeof(BaseGame) ||
                            parameters[0].ParameterType.IsSubclassOf(typeof(BaseGame))) &&
                           (parameters[1].ParameterType == typeof(IBaseGameComponentContainer) ||
                            parameters[1].ParameterType.ImplementsInterface(typeof(IBaseGameComponentContainer)));
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

            return (BaseGameComponent)ctor.Invoke(new object[] { game, parent });
        }

        /// <summary>
        /// Creates a new <see cref="BaseGameComponent"/> and adds it to the <see cref="parent"/>.
        /// The component must have a constructor (public or non-public) of the form <see cref="BaseGameComponent(BaseGame,IBaseGameComponentContainer)"/>.
        /// </summary>
        /// <typeparam name="T">Type of the component.</typeparam>
        /// <param name="game">The <see cref="BaseGame"/> instance.</param>
        /// <param name="parent">The parent of created component.</param>
        /// <returns>Created component.</returns>
        [NotNull]
        public static T CreateAndAddTo<T>([NotNull] BaseGame game, [NotNull] IBaseGameComponentContainer parent) where T : BaseGameComponent {
            var t = typeof(T);
            var c = CreateAndAddTo(game, parent, t);
            return (T)c;
        }

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
