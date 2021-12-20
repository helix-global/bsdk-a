using System;
using System.ServiceProcess;

public class LocalService : ILocalClient
    {
    public Int32 Main(String[] args)
        {
        ServiceBase[] ServicesToRun;
        ServicesToRun = new ServiceBase[]
            {
            new srv.Service()
            };
        ServiceBase.Run(ServicesToRun);
        return 0;
        }

    void IDisposable.Dispose()
        {
        }
    }
