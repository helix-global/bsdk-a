using System;
using System.Threading;

namespace BinaryStudio.Utilities
    {
    public abstract class ReusableResourceStoreBase<TResource> where TResource : class
        {
        private TResource resource;

        protected TResource AcquireCore()
            {
            return Interlocked.Exchange(ref resource, default(TResource));
            }

        internal void ReleaseCore(TResource value)
            {
            if (!Cleanup(value))
                return;
            Interlocked.Exchange(ref resource, value);
            }

        protected virtual Boolean Cleanup(TResource value)
            {
            return true;
            }
        }
    }