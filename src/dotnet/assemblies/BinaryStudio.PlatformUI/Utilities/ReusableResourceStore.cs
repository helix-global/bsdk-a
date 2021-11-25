using System;

namespace BinaryStudio.Utilities
    {
    public abstract class ReusableResourceStore<TResource, TConstructorParameter> : ReusableResourceStoreBase<TResource> where TResource : class
        {
        public ReusableResourceHolder<TResource> Acquire(TConstructorParameter constructorParameter)
            {
            var resource = AcquireCore();
            if (resource == null || !CanReuse(resource, constructorParameter))
                resource = Allocate(constructorParameter);
            return new ReusableResourceHolder<TResource>(this, resource);
            }

        protected abstract TResource Allocate(TConstructorParameter constructorParameter);

        protected virtual Boolean CanReuse(TResource value, TConstructorParameter parameter)
            {
            return true;
            }
        }
    }