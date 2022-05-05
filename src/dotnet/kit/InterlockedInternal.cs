using System.Threading;

internal class InterlockedInternal<T>
    {
    private T r;
    private readonly ReaderWriterLockSlim o = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

    public InterlockedInternal()
        {
        }

    public InterlockedInternal(T source)
        {
        Value = source;
        }

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
