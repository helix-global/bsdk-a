using System;
using System.ServiceProcess;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using log4net;
using Microsoft.Win32;

namespace srv
    {
    public partial class Service : ServiceBase, ILogger
        {
        private static readonly ILog log = LogManager.GetLogger(nameof(Service));
        public CRYPT_PROVIDER_TYPE ProviderType { get;set; }
        public Service()
            {
            InitializeComponent();
            ProviderType = CRYPT_PROVIDER_TYPE.PROV_GOST_2012_256;
            }

        /// <summary>When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.</summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(String[] args)
            {
            var Is64Bit = Environment.Is64BitProcess;
            log.Info($"Keys {{{ProviderType}}}:");
            log.Info("  User Keys:");
            var j = 0;
            using (var context = new CryptographicContext(this, ProviderType, CryptographicContextFlags.CRYPT_SILENT|CryptographicContextFlags.CRYPT_VERIFYCONTEXT)) {
                foreach (var i in context.Keys) {
                    var handle = Is64Bit
                        ? ((Int64)i.Handle).ToString("X16")
                        : ((Int32)i.Handle).ToString("X8");
                    log.Info($"    {{{j}}}:{{{handle}}}:{i.Container}");
                    j++;
                    }
                }
            log.Info("  Machine Keys:");
            j = 0;
            using (var context = new CryptographicContext(this, ProviderType, CryptographicContextFlags.CRYPT_SILENT|CryptographicContextFlags.CRYPT_VERIFYCONTEXT|CryptographicContextFlags.CRYPT_MACHINE_KEYSET)) {
                foreach (var i in context.Keys) {
                    var handle = Is64Bit
                        ? ((Int64)i.Handle).ToString("X16")
                        : ((Int32)i.Handle).ToString("X8");
                    log.Info($"    {{{j}}}:{{{handle}}}:{i.Container}");
                    j++;
                    }
                }
            }

        protected override void OnStop()
            {
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
