using System;

public class LocalService : ILocalClient
    {
    public Int32 Main(String[] args)
        {
        return 0;
        }

    void IDisposable.Dispose()
        {
        }
    }
