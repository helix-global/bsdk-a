using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BinaryStudio.Diagnostics.Logging;
using Kit;

namespace kit
    {
    internal class ConsoleLogger : ILogger
        {
        private static readonly IDictionary<LogLevel,ConsoleColor?> colors = new Dictionary<LogLevel, ConsoleColor?>{
            { LogLevel.Debug,       ConsoleColor.Gray    },
            { LogLevel.Critical,    ConsoleColor.Red     },
            { LogLevel.Error,       ConsoleColor.DarkRed },
            { LogLevel.Trace,       ConsoleColor.Gray    },
            { LogLevel.Warning,     ConsoleColor.Yellow  },
            { LogLevel.Information, null },
            { LogLevel.None,        null }
            };

        public Boolean IsEnabled(LogLevel loglevel)
            {
            return true;
            }

        public void Log(LogLevel loglevel, String message)
            {
            var color = colors[loglevel];
            using ((color != null)
                ? new ConsoleColorScope(color.Value)
                : null)
                {
                Console.WriteLine(message);
                }
            }

        void ILogger.Log(LogLevel loglevel, String format, params Object[] args)
            {
            Log(loglevel, String.Format(format, args));
            }
        }
    }