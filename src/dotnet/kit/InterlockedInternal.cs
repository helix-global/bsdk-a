using System;
using System.Threading;

internal class InterlockedInternal<T>
    {
    private T r;
    private readonly ReaderWriterLockSlim o = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

    public T Value
        {
        get
            {
            o.EnterReadLock();
            try
                {
                return r;
                }
            finally
                {
                o.ExitReadLock();
                }
            }
        set
            {
            o.EnterWriteLock();
            try
                {
                r = value;
                }
            finally
                {
                o.ExitWriteLock();
                }
            }
        }
    }
