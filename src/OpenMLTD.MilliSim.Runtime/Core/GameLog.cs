using System;
using System.Reflection;
using JetBrains.Annotations;
using log4net;

namespace OpenMLTD.MilliSim.Core {
    public sealed class GameLog {

        private GameLog([NotNull] Assembly assembly, [NotNull] string name) {
            _log = LogManager.GetLogger(assembly, name);
        }

        public static void Initialize([NotNull] string loggerName) {
            Initialize(Assembly.GetEntryAssembly(), loggerName);
        }

        public static void Initialize([NotNull] Assembly assembly, [NotNull] string loggerName) {
            if (_isInitialized) {
                return;
            }

            _isInitialized = true;
            _instance = new GameLog(assembly, loggerName);
        }

        public static bool Enabled { get; set; }

        #region log:debug
        public static void Debug([NotNull] string message) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsDebugEnabled) {
                log.Debug(message);
            }
        }
        public static void Debug(object message) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsDebugEnabled) {
                log.Debug(message);
            }
        }

        public static void Debug(object message, Exception exception) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsDebugEnabled) {
                log.Debug(message, exception);
            }
        }

        [StringFormatMethod("format")]
        public static void Debug([NotNull] string format, object arg0) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsDebugEnabled) {
                log.DebugFormat(format, arg0);
            }
        }

        [StringFormatMethod("format")]
        public static void Debug([NotNull] string format, object arg0, object arg1) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsDebugEnabled) {
                log.DebugFormat(format, arg0, arg1);
            }
        }

        [StringFormatMethod("format")]
        public static void Debug([NotNull] string format, object arg0, object arg1, object arg2) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsDebugEnabled) {
                log.DebugFormat(format, arg0, arg1, arg2);
            }
        }

        [StringFormatMethod("format")]
        public static void Debug([NotNull] string format, params object[] args) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsDebugEnabled) {
                log.DebugFormat(format, args);
            }
        }

        [StringFormatMethod("format")]
        public static void Debug([NotNull] IFormatProvider formatProvider, string format, params object[] args) {
            if (!Enabled || _instance == null) {
                return;
            }


            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsDebugEnabled) {
                log.DebugFormat(formatProvider, format, args);
            }
        }
        #endregion

        #region log:info
        public static void Info([NotNull] string message) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsInfoEnabled) {
                log.Info(message);
            }
        }

        public static void Info(object message) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsInfoEnabled) {
                log.Info(message);
            }
        }

        public static void Info(object message, Exception exception) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsInfoEnabled) {
                log.Info(message, exception);
            }
        }

        [StringFormatMethod("format")]
        public static void Info([NotNull] string format, object arg0) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsInfoEnabled) {
                log.InfoFormat(format, arg0);
            }
        }

        [StringFormatMethod("format")]
        public static void Info([NotNull] string format, object arg0, object arg1) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsInfoEnabled) {
                log.InfoFormat(format, arg0, arg1);
            }
        }

        [StringFormatMethod("format")]
        public static void Info([NotNull] string format, object arg0, object arg1, object arg2) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsInfoEnabled) {
                log.InfoFormat(format, arg0, arg1, arg2);
            }
        }

        [StringFormatMethod("format")]
        public static void Info([NotNull] string format, params object[] args) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsInfoEnabled) {
                log.InfoFormat(format, args);
            }
        }

        [StringFormatMethod("format")]
        public static void Info([NotNull] IFormatProvider formatProvider, string format, params object[] args) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsInfoEnabled) {
                log.InfoFormat(formatProvider, format, args);
            }
        }
        #endregion

        #region log:warn
        public static void Warn([NotNull] string message) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsWarnEnabled) {
                log.Warn(message);
            }
        }

        public static void Warn(object message) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsWarnEnabled) {
                log.Warn(message);
            }
        }

        public static void Warn(object message, Exception exception) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsWarnEnabled) {
                log.Warn(message, exception);
            }
        }

        [StringFormatMethod("format")]
        public static void Warn([NotNull] string format, object arg0) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsWarnEnabled) {
                log.WarnFormat(format, arg0);
            }
        }

        [StringFormatMethod("format")]
        public static void Warn([NotNull] string format, object arg0, object arg1) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsWarnEnabled) {
                log.WarnFormat(format, arg0, arg1);
            }
        }

        [StringFormatMethod("format")]
        public static void Warn([NotNull] string format, object arg0, object arg1, object arg2) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsWarnEnabled) {
                log.WarnFormat(format, arg0, arg1, arg2);
            }
        }

        [StringFormatMethod("format")]
        public static void Warn([NotNull] string format, params object[] args) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsWarnEnabled) {
                log.WarnFormat(format, args);
            }
        }

        [StringFormatMethod("format")]
        public static void Warn([NotNull] IFormatProvider formatProvider, string format, params object[] args) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsWarnEnabled) {
                log.WarnFormat(formatProvider, format, args);
            }
        }
        #endregion

        #region log:error
        public static void Error([NotNull] string message) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsErrorEnabled) {
                log.Error(message);
            }
        }

        public static void Error(object message) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsErrorEnabled) {
                log.Error(message);
            }
        }

        public static void Error(object message, Exception exception) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsErrorEnabled) {
                log.Error(message, exception);
            }
        }

        [StringFormatMethod("format")]
        public static void Error([NotNull] string format, object arg0) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsErrorEnabled) {
                log.ErrorFormat(format, arg0);
            }
        }

        [StringFormatMethod("format")]
        public static void Error([NotNull] string format, object arg0, object arg1) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsErrorEnabled) {
                log.ErrorFormat(format, arg0, arg1);
            }
        }

        [StringFormatMethod("format")]
        public static void Error([NotNull] string format, object arg0, object arg1, object arg2) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsErrorEnabled) {
                log.ErrorFormat(format, arg0, arg1, arg2);
            }
        }

        [StringFormatMethod("format")]
        public static void Error([NotNull] string format, params object[] args) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsErrorEnabled) {
                log.ErrorFormat(format, args);
            }
        }

        [StringFormatMethod("format")]
        public static void Error([NotNull] IFormatProvider formatProvider, string format, params object[] args) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsErrorEnabled) {
                log.ErrorFormat(formatProvider, format, args);
            }
        }
        #endregion

        #region log:fatal
        public static void Fatal([NotNull] string message) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsFatalEnabled) {
                log.Fatal(message);
            }
        }

        public static void Fatal(object message) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsFatalEnabled) {
                log.Fatal(message);
            }
        }

        public static void Fatal(object message, Exception exception) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsFatalEnabled) {
                log.Fatal(message, exception);
            }
        }

        [StringFormatMethod("format")]
        public static void Fatal([NotNull] string format, object arg0) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsFatalEnabled) {
                log.FatalFormat(format, arg0);
            }
        }

        [StringFormatMethod("format")]
        public static void Fatal([NotNull] string format, object arg0, object arg1) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsFatalEnabled) {
                log.FatalFormat(format, arg0, arg1);
            }
        }

        [StringFormatMethod("format")]
        public static void Fatal([NotNull] string format, object arg0, object arg1, object arg2) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsFatalEnabled) {
                log.FatalFormat(format, arg0, arg1, arg2);
            }
        }

        [StringFormatMethod("format")]
        public static void Fatal([NotNull] string format, params object[] args) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsFatalEnabled) {
                log.FatalFormat(format, args);
            }
        }

        [StringFormatMethod("format")]
        public static void Fatal([NotNull] IFormatProvider formatProvider, string format, params object[] args) {
            if (!Enabled || _instance == null) {
                return;
            }

            var log = _instance._log;

            if (log == null) {
                return;
            }

            if (log.IsFatalEnabled) {
                log.FatalFormat(formatProvider, format, args);
            }
        }
        #endregion

        private static GameLog _instance;
        private static bool _isInitialized;

        private readonly ILog _log;

    }
}
