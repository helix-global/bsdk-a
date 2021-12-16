using System;

namespace BinaryStudio.Diagnostics.Logging
    {
    public class EmptyLogger : ILogger
        {
        Boolean ILogger.IsEnabled(LogLevel loglevel)
            {
            return false;
            }

        void ILogger.Log(LogLevel loglevel, String message)
            {
            }

        void ILogger.Log(LogLevel loglevel, String format, params Object[] args)
            {
            }
        }
    }