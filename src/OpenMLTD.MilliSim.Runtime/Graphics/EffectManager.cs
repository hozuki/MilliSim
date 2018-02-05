using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Graphics {
    public sealed class EffectManager : DisposableBase {

        internal EffectManager([NotNull] BaseGame game) {
            _game = game;
        }

        public T Register<T>([NotNull] string effectFile)
            where T : Effect {
            var t = typeof(T);

            return (T)Register(t, effectFile);
        }

        public Effect Register([NotNull] Type effectType, [NotNull] string effectFile) {
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

        public T Get<T>()
            where T : Effect {
            return (T)Get(typeof(T));
        }

        public Effect Get([NotNull] Type effectType) {
            return _effects[effectType];
        }

        public bool Remove<T>()
            where T : Effect {
            return Remove(typeof(T));
        }

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

        private static Effect CreateEffect([NotNull] Type effectType, [NotNull] GraphicsDevice graphicsDevice, [NotNull] byte[] bytecode) {
            if (effectType != typeof(Effect) && !effectType.IsSubclassOf(typeof(Effect))) {
                throw new ArgumentException($"Type {effectType} is not an effect.", nameof(effectType));
            }

            var obj = Activator.CreateInstance(effectType, graphicsDevice, bytecode);
            var instance = (Effect)obj;

            return instance;
        }

        private readonly BaseGame _game;
        private readonly Dictionary<Type, Effect> _effects = new Dictionary<Type, Effect>();

    }
}
