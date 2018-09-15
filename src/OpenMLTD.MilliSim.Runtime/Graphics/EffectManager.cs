using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Graphics {
    /// <inheritdoc />
    /// <summary>
    /// A manager for <see cref="Effect"/>s.
    /// </summary>
    public sealed class EffectManager : DisposableBase {

        /// <summary>
        /// Creates a new <see cref="EffectManager"/> instance.
        /// </summary>
        /// <param name="game">The base game.</param>
        internal EffectManager([NotNull] BaseGame game) {
            _game = game;
        }

        /// <summary>
        /// Registers an effect instance as a singleton.
        /// </summary>
        /// <param name="effectInstance">The effect instance.</param>
        /// <returns><see langword="true"/> if there was no registered effects with the same type of the effect that is to be registered, otherwise <see langword="false"/>.</returns>
        public bool RegisterSingleton([NotNull] Effect effectInstance) {
            var effectType = effectInstance.GetType();

            if (_effects.ContainsKey(effectType)) {
                return false;
            }

            _effects[effectType] = effectInstance;

            return true;
        }

        /// <summary>
        /// Registers an effect instance as a singleton.
        /// </summary>
        /// <param name="effectFile">Path to the effect (.fx) file.</param>
        /// <typeparam name="T">The type of the effect.</typeparam>
        /// <returns>Registered effect instance.</returns>
        [NotNull]
        public T RegisterSingleton<T>([NotNull] string effectFile)
            where T : Effect {
            var t = typeof(T);

            return (T)RegisterSingleton(t, effectFile);
        }

        /// <summary>
        /// Registers an effect instance as a singleton.
        /// </summary>
        /// <param name="effectType">The type of the effect.</param>
        /// <param name="effectFile">Path to the effect (.fx) file.</param>
        /// <returns>Registered effect instance.</returns>
        [NotNull]
        public Effect RegisterSingleton([NotNull] Type effectType, [NotNull] string effectFile) {
            if (_effects.ContainsKey(effectType)) {
                return _effects[effectType];
            }

            var graphicsBackend = BaseGame.GraphicsBackend;

            string backendID;

            switch (graphicsBackend) {
                case GraphicsBackend.Direct3D11:
                    backendID = "dx11";
                    break;
                case GraphicsBackend.OpenGL:
                    backendID = "ogl";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var translatedFileName = $"{effectFile}.{backendID}.mgfxo";
            var bytecode = File.ReadAllBytes(translatedFileName);

            var graphicsDevice = _game.GraphicsDevice;

            var effect = CreateEffect(effectType, graphicsDevice, bytecode);

            _effects[effectType] = effect;

            return effect;
        }

        /// <summary>
        /// Gets an effect object by its type.
        /// </summary>
        /// <typeparam name="T">The type of the effect.</typeparam>
        /// <returns>Retrieved effect object.</returns>
        [NotNull]
        public T Get<T>()
            where T : Effect {
            return (T)Get(typeof(T));
        }

        /// <summary>
        /// Gets an effect object by its type.
        /// </summary>
        /// <param name="effectType">The type of the effect.</param>
        /// <returns>Retrieved effect object.</returns>
        [NotNull]
        public Effect Get([NotNull] Type effectType) {
            return _effects[effectType];
        }

        /// <summary>
        /// Checks whether an effect instance of specified type is already registered.
        /// </summary>
        /// <param name="effectType">The type of the effect.</param>
        /// <returns><see langword="true"/> if there is such an effect object, otherwise <see langword="false"/>.</returns>
        public bool Contains([NotNull] Type effectType) {
            return _effects.ContainsKey(effectType);
        }

        /// <summary>
        /// Checks whether an effect instance of specified type is already registered.
        /// </summary>
        /// <typeparam name="T">The type of the effect.</typeparam>
        /// <returns><see langword="true"/> if there is such an effect object, otherwise <see langword="false"/>.</returns>
        public bool Contains<T>()
            where T : Effect {
            return Contains(typeof(T));
        }

        /// <summary>
        /// Try to retrieve a registered effect instance by type.
        /// </summary>
        /// <param name="effectType">The type of the effect.</param>
        /// <param name="effect">Retrieved effect object.</param>
        /// <returns><see langword="true"/> if there is such an effect object, otherwise <see langword="false"/>.</returns>
        public bool TryGet([NotNull] Type effectType, [CanBeNull] out Effect effect) {
            return _effects.TryGetValue(effectType, out effect);
        }

        /// <summary>
        /// Try to retrieve a registered effect instance by type.
        /// </summary>
        /// <typeparam name="T">The type of the effect.</typeparam>
        /// <param name="effect">Retrieved effect object.</param>
        /// <returns><see langword="true"/> if there is such an effect object, otherwise <see langword="false"/>.</returns>
        public bool TryGet<T>([CanBeNull] out T effect)
            where T : Effect {
            var b = TryGet(typeof(T), out var e);
            effect = (T)e;
            return b;
        }

        /// <summary>
        /// Removes a registered effect object by type.
        /// </summary>
        /// <typeparam name="T">The type of the effect.</typeparam>
        /// <returns><see langword="true"/> if an effect object is removed, otherwise <see langword="false"/>.</returns>
        public bool Remove<T>()
            where T : Effect {
            return Remove(typeof(T));
        }

        /// <summary>
        /// Removes a registered effect object by type.
        /// </summary>
        /// <param name="effectType">The type of the effect.</param>
        /// <returns><see langword="true"/> if an effect object is removed, otherwise <see langword="false"/>.</returns>
        public bool Remove([NotNull] Type effectType) {
            if (!_effects.ContainsKey(effectType)) {
                return false;
            }

            var effect = _effects[effectType];
            _effects.Remove(effectType);

            effect.Dispose();

            return true;
        }

        protected override void Dispose(bool disposing) {
            foreach (var effect in _effects.Values) {
                effect.Dispose();
            }

            _effects.Clear();
        }

        [NotNull]
        private static Effect CreateEffect([NotNull] Type effectType, [NotNull] GraphicsDevice graphicsDevice, [NotNull] byte[] bytecode) {
            if (effectType != typeof(Effect) && !effectType.IsSubclassOf(typeof(Effect))) {
                throw new ArgumentException($"Type {effectType} is not an effect.", nameof(effectType));
            }

            // Use the (GraphicsDevice, byte[]) constructor.
            var obj = Activator.CreateInstance(effectType, graphicsDevice, bytecode);
            var instance = (Effect)obj;

            return instance;
        }

        private readonly BaseGame _game;
        private readonly Dictionary<Type, Effect> _effects = new Dictionary<Type, Effect>();

    }
}
