using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using PerformanceCounter = OpenMLTD.MilliSim.Core.PerformanceCounter;

namespace OpenMLTD.MilliSim.Foundation {
    public abstract class GameBase : DisposableBase {

        protected GameBase() {
            _timeLock = new SimpleUsingLock();
            _suspensionLock = new SimpleUsingLock();
        }

        public event EventHandler<EventArgs> WindowLoad;

        public virtual void Update([NotNull] GameTime gameTime) {
            Root.Update(gameTime);
        }

        public abstract void Draw([NotNull] GameTime gameTime);

        public void Run() => Run(new string[0]);

        public void Run<TWindow>() where TWindow : GameWindow => Run<TWindow>(new string[0]);

        public void Run(string[] args) {
            Run<GameWindow>(args);
        }

        public void Run<TWindow>(string[] args) where TWindow : GameWindow {
            if (_isAlreadyRun) {
                throw new InvalidOperationException("The game has already been run once.");
            }

            _isAlreadyRun = true;

            var t = typeof(TWindow);
            var ctor = t.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(GameBase) }, new ParameterModifier[0]);
            if (ctor == null) {
                throw new MissingMemberException(t.Name, $".ctor({typeof(GameBase)})");
            }

            using (var window = (TWindow)ctor.Invoke(new object[] { this })) {
                Window = window;
                // Must create the control before render thread starts, or a InvalidCall exception will be thrown.
                window.CreateControl();

                Root = CreateRootElement();
                CreateElements();

                using (var audioManager = CreateAudioManager()) {
                    BaseAudioManager = audioManager;

                    using (var renderer = CreateRenderer()) {
                        renderer.Initialize();
                        BaseRenderer = renderer;

                        OnInitialize();

                        _exitingEvent = new ManualResetEvent(false);

                        _workerThread = new Thread(WorkerThreadProc);
                        _workerThread.IsBackground = true;

                        window.Closed += GameWindowOnClosed;
                        OnWindowLoad(EventArgs.Empty);

                        _workerThread.Start(window);

                        window.ShowDialog();

                        _exitingEvent.WaitOne();
                        _exitingEvent.Dispose();
                    }
                }
            }
        }

        public void ResetTick() {
            using (_timeLock.NewWriteLock()) {
                _startTick = PerformanceCounter.GetCurrent();
                _lastTick = _startTick;
            }
        }

        public bool IsSuspended {
            get {
                using (_suspensionLock.NewReadLock()) {
                    return _isSuspended;
                }
            }
            private set {
                using (_suspensionLock.NewWriteLock()) {
                    _isSuspended = value;
                }
            }
        }

        public void Suspend() {
            if (IsSuspended) {
                return;
            }
            IsSuspended = true;
        }

        public void Resume() {
            if (!IsSuspended) {
                return;
            }
            IsSuspended = false;
        }

        [NotNull]
        public IComponentContainer Root { get; private set; }

        [NotNull]
        public GameTime Time {
            get {
                using (_timeLock.NewReadLock()) {
                    return _gameTime;
                }
            }
            set {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }
                using (_timeLock.NewWriteLock()) {
                    _gameTime = value;
                }
            }
        }

        public GameWindow Window { get; private set; }

        public virtual string Title { get; } = "Game";

        protected RendererBase BaseRenderer { get; private set; }

        protected AudioManagerBase BaseAudioManager { get; private set; }

        /// <summary>
        /// Invoke an <see cref="Action"/> on worker thread.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to invoke.</param>
        public void Invoke(Action action) {
            _actionQueue.Enqueue((_ => action(), null));
        }

        /// <summary>
        /// Invoke an <see cref="Action{T}"/> on worker thread.
        /// </summary>
        /// <param name="action">The <see cref="Action{T}"/> to invoke.</param>
        /// <param name="state">The state object (action argument) to pass to the <see cref="Action{T}"/>.</param>
        public void Invoke(Action<object> action, object state) {
            _actionQueue.Enqueue((action, state));
        }

        protected abstract RendererBase CreateRenderer();

        protected abstract AudioManagerBase CreateAudioManager();

        protected abstract void CreateElements();

        [NotNull]
        protected abstract IComponentContainer CreateRootElement();

        protected virtual void OnInitialize() {
            Root.OnInitialize();
        }

        protected virtual void OnWindowLoad(EventArgs e) {
            WindowLoad?.Invoke(this, e);
        }

        protected override void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }

            Window?.Dispose();
            _timeLock.Dispose();
            _suspensionLock.Dispose();
        }

        private void WorkerThreadProc(object param) {
            ResetTick();
            while (_shouldContinue) {
                var isSuspended = IsSuspended;
                GameTime gameTime;

                if (!isSuspended) {
                    var nowTick = PerformanceCounter.GetCurrent();
                    double delta, total;
                    using (_timeLock.NewReadLock()) {
                        delta = PerformanceCounter.GetDuration(_lastTick, nowTick);
                        total = PerformanceCounter.GetDuration(_startTick, nowTick);
                    }
                    gameTime = new GameTime(TimeSpan.FromMilliseconds(delta), TimeSpan.FromMilliseconds(total));
                    Update(gameTime);
                    Time = gameTime;

                    _lastTick = nowTick;
                } else {
                    gameTime = Time;
                }

                Draw(gameTime);

                if (!isSuspended) {
                    while (_actionQueue.Count > 0) {
                        var item = _actionQueue.Dequeue();
                        item.Action(item.State);
                    }
                }
            }
            _exitingEvent.Set();
        }

        private void GameWindowOnClosed(object sender, EventArgs e) {
            _shouldContinue = false;
            Window.Closed -= GameWindowOnClosed;
        }

        private volatile bool _shouldContinue = true;

        private long _startTick;
        private long _lastTick;
        private bool _isSuspended;

        private GameTime _gameTime = GameTime.Zero;

        private Thread _workerThread;
        // A simple action queue. Used to mimic the 'Invoke' pattern on a specific thread.
        // Enlightened from https://stackoverflow.com/questions/3481075/invoke-a-delegate-on-a-specific-thread-c-sharp.
        private readonly Queue<(Action<object> Action, object State)> _actionQueue = new Queue<(Action<object>, object)>();

        private ManualResetEvent _exitingEvent;
        private readonly SimpleUsingLock _timeLock;
        private readonly SimpleUsingLock _suspensionLock;

        private bool _isAlreadyRun;

    }
}
