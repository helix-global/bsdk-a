using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace srv
    {
    static class Program
        {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
            {
            #if D
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service()
            };
            ServiceBase.Run(ServicesToRun);
            #endif
            }
        }
    }
