using System;

namespace BinaryStudio.Utilities
    {
    public struct ReusableResourceHolder<TResource> : IDisposable where TResource : class
        {
        private readonly ReusableResourceStoreBase<TResource> store;
        private TResource resource;

        public TResource Resource
            {
            get
                {
                return resource;
                }
            }

        internal ReusableResourceHolder(ReusableResourceStoreBase<TResource> store, TResource value)
            {
            this.store = store;
            resource = value;
            }

        public void Dispose()
            {
            store.ReleaseCore(resource);
            resource = default(TResource);
            }
        }
    }