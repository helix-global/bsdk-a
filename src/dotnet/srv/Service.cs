using System;
using System.ServiceProcess;
using System.Threading;
using BinaryStudio.Diagnostics.Logging;
using log4net;
using Microsoft.Win32;

namespace srv
    {
    public partial class Service : ServiceBase
        {
        private static readonly ILogger logger = new ServiceLogger(LogManager.GetLogger(nameof(Service)));
        public CRYPT_PROVIDER_TYPE ProviderType { get;set; }
        private static Thread thread;
        private Boolean stop;

        public Service()
            {
            InitializeComponent();
            ProviderType = CRYPT_PROVIDER_TYPE.PROV_GOST_2012_256;
            }

        /// <summary>When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.</summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(String[] args)
            {
            logger.Log(LogLevel.Information, "Starting...");
            thread = new Thread(()=>{
                try
                    {
                    using (new ServiceLogic(50000)) {
                        logger.Log(LogLevel.Information, "Started");
                        while (!stop)
                            {
                            Thread.Sleep(5000);
                            }
                        }
                    }
                catch (Exception e)
                    {
                    logger.Log(LogLevel.Critical, e.ToString());
                    stop = true;
                    }
                });
            thread.Start();
            }

        /// <summary>When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.</summary>
        protected override void OnStop()
            {
            logger.Log(LogLevel.Information, "Stopping...");
            stop = true;
            thread.Join();
            logger.Log(LogLevel.Information, "Stopped");
            }
        }
    }
