using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpenMLTD.MilliSim.Audio;
using OpenMLTD.MilliSim.Configuration;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Globalization;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Foundation {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public abstract class BaseGame : Game {

        protected BaseGame([NotNull] string contentRootDirectory, [NotNull] BasePluginManager pluginManager) {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            _audioManager = new AudioManager();
            _pluginManager = pluginManager;

            Content.RootDirectory = contentRootDirectory;
        }

        public BasePluginManager PluginManager => _pluginManager;

        public SpriteBatch SpriteBatch => _spriteBatch;

        public GraphicsDeviceManager GraphicsDeviceManager => _graphicsDeviceManager;

        public AudioManager AudioManager => _audioManager;

        public abstract Stage Stage { get; }

        public abstract ConfigurationStore ConfigurationStore { get; }

        public abstract CultureSpecificInfo CultureSpecificInfo { get; }

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

            // TODO: use this.Content to load your game content here
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
            // TODO: Unload any non ContentManager content here
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

                GraphicsDevice.Clear(Color.CornflowerBlue);

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
                _pluginManager?.Dispose();

                _spriteBatch = null;
                _graphicsDeviceManager = null;
                _audioManager = null;
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
        private BasePluginManager _pluginManager;

    }
}
