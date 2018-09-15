using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Overlay;
using OpenMLTD.MilliSim.Audio;
using OpenMLTD.MilliSim.Configuration;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Globalization;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Foundation {
    /// <summary>
    /// This is the main type for your game.
    /// This class must be inherited.
    /// </summary>
    public abstract class BaseGame : Game {

        protected BaseGame([NotNull] string contentRootDirectory, [NotNull] BasePluginManager pluginManager) {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            _audioManager = new AudioManager();
            _effectManager = new EffectManager(this);
            _fontManager = new FontManager();
            _pluginManager = pluginManager;

            Content.RootDirectory = contentRootDirectory;
        }

        /// <summary>
        /// Gets the plugin manager.
        /// </summary>
        [NotNull]
        public BasePluginManager PluginManager => _pluginManager;

        /// <summary>
        /// Gets the main sprite batch.
        /// </summary>
        [NotNull]
        public SpriteBatch SpriteBatch => _spriteBatch;

        /// <summary>
        /// Gets the graphics device manager.
        /// </summary>
        [NotNull]
        public GraphicsDeviceManager GraphicsDeviceManager => _graphicsDeviceManager;

        /// <summary>
        /// Gets the audio manager.
        /// </summary>
        [NotNull]
        public AudioManager AudioManager => _audioManager;

        /// <summary>
        /// Gets the effect manager.
        /// </summary>
        [NotNull]
        public EffectManager EffectManager => _effectManager;

        /// <summary>
        /// Gets the font manager.
        /// </summary>
        [NotNull]
        public FontManager FontManager => _fontManager;

        /// <summary>
        /// Gets the main stage. This property must be overridden.
        /// </summary>
        [NotNull]
        public abstract Stage Stage { get; }

        /// <summary>
        /// Gets the configuration store. This property must be overridden.
        /// </summary>
        [NotNull]
        public abstract BaseConfigurationStore ConfigurationStore { get; }

        /// <summary>
        /// Gets the culture-specific information. This property must be overridden.
        /// </summary>
        [NotNull]
        public abstract CultureSpecificInfo CultureSpecificInfo { get; }

        /// <summary>
        /// Gets or sets the graphics backend.
        /// This property must be set before starting the game, and can only be set once.
        /// </summary>
        public static GraphicsBackend GraphicsBackend {
            get => _graphicsBackend;
            set {
                if (_graphicsBackend != GraphicsBackend.Unknown) {
                    throw new InvalidOperationException();
                }

                switch (value) {
                    case GraphicsBackend.Direct3D11:
                    case GraphicsBackend.OpenGL:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }

                _graphicsBackend = value;
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            base.Initialize();

            GraphicsDevice.PresentationParameters.PresentationInterval = PresentInterval.One;

            OnCreateComponents();

            foreach (var component in Components) {
                component.Initialize();
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (var component in Components) {
                if (component is IBaseGameComponent c) {
                    c.LoadContents();
                }
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            foreach (var component in Components) {
                if (component is IBaseGameComponent c) {
                    c.UnloadContents();
                }
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            using (_disposeLock) {
                if (_isDisposed) {
                    return;
                }

                base.Update(gameTime);

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                    Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                    Exit();
                }

                foreach (var component in Components) {
                    if (component is IUpdateable u) {
                        u.Update(gameTime);
                    }
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            using (_disposeLock) {
                if (_isDisposed) {
                    return;
                }

                GraphicsDevice.Clear(Color.Black);

                foreach (var component in Components) {
                    if (component is IDrawable d) {
                        d.Draw(gameTime);
                    }
                }
            }
        }

        protected override void Dispose(bool disposing) {
            using (_disposeLock) {
                _isDisposed = true;

                foreach (var component in Components) {
                    if (component is IDisposable disposable) {
                        disposable.Dispose();
                    }
                }

                Components.Clear();

                _spriteBatch?.Dispose();
                _graphicsDeviceManager?.Dispose();
                _audioManager?.Dispose();
                _effectManager?.Dispose();
                _fontManager?.Dispose();
                _pluginManager?.Dispose();

                _spriteBatch = null;
                _graphicsDeviceManager = null;
                _audioManager = null;
                _effectManager = null;
                _fontManager = null;
                _pluginManager = null;
            }

            base.Dispose(disposing);
        }

        protected virtual void OnCreateComponents() {
        }

        private bool _isDisposed;
        private readonly SimpleUsingLock _disposeLock = new SimpleUsingLock();
        private SpriteBatch _spriteBatch;
        private GraphicsDeviceManager _graphicsDeviceManager;
        private AudioManager _audioManager;
        private EffectManager _effectManager;
        private FontManager _fontManager;
        private BasePluginManager _pluginManager;

        private static GraphicsBackend _graphicsBackend;

    }
}
