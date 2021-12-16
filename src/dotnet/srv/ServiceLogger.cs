using System;
using BinaryStudio.Diagnostics.Logging;
using log4net;

namespace srv
    {
    public class ServiceLogger : ILogger
        {
        private readonly ILog log;
        public ServiceLogger(ILog log)
            {
            this.log = log;
            }

        Boolean ILogger.IsEnabled(LogLevel loglevel) {
            switch (loglevel)
                {
                case LogLevel.Trace:       return log.IsDebugEnabled;
                case LogLevel.Debug:       return log.IsDebugEnabled;
                case LogLevel.Information: return log.IsInfoEnabled;
                case LogLevel.Warning:     return log.IsWarnEnabled;
                case LogLevel.Error:       return log.IsErrorEnabled;
                case LogLevel.Critical:    return log.IsFatalEnabled;
                case LogLevel.None: break;
                default: throw new ArgumentOutOfRangeException(nameof(loglevel), loglevel, null);
                }
            return false;
            }

        public void Log(LogLevel loglevel, String message) {
            switch (loglevel)
                {
                case LogLevel.Trace:       log.Debug(message); break;
                case LogLevel.Debug:       log.Debug(message); break;
                case LogLevel.Information: log.Info(message);  break;
                case LogLevel.Warning:     log.Warn(message);  break;
                case LogLevel.Error:       log.Error(message); break;
                case LogLevel.Critical:    log.Fatal(message); break;
                case LogLevel.None: break;
                default: throw new ArgumentOutOfRangeException(nameof(loglevel), loglevel, null);
                }
            }

        void ILogger.Log(LogLevel loglevel, String format, params Object[] args)
            {
            Log(loglevel, String.Format(format, args));
            }
        }
    }