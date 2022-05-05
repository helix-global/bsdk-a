using System;
using System.Diagnostics;

namespace BinaryStudio.Diagnostics.Logging
    {
    public class DefaultLogger : ILogger
        {
        public virtual Boolean IsEnabled(LogLevel loglevel)
            {
            #if DEBUG
            if (loglevel == LogLevel.Debug) { return true; }
            #endif
            #if TRACE
            if (loglevel == LogLevel.Trace) { return true; }
            #endif
            return false;
            }

        public virtual void Log(LogLevel loglevel, String message) {
            switch (loglevel) {
                case LogLevel.Debug: Debug.WriteLine(message); break;
                case LogLevel.Trace: Trace.WriteLine(message); break;
                }
            }

        void ILogger.Log(LogLevel loglevel, String format, params Object[] args)
            {
            Log(loglevel, String.Format(format, args));
            }

        void ILogger.Log(LogLevel loglevel, Exception e)
            {
            Log(loglevel, $"{Environment.NewLine}{Exceptions.ToString(e)}");
            }
        }
    }