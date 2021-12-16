using System;

namespace BinaryStudio.Diagnostics.Logging
    {
    public interface ILogger
        {
        Boolean IsEnabled(LogLevel loglevel);
        void Log(LogLevel loglevel, String message);
        void Log(LogLevel loglevel, String format, params Object[] args);
        }
    }