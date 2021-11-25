using System;
using System.Text;
using BinaryStudio.PlatformUI.Shell;

namespace BinaryStudio.Utilities
    {
    public sealed class ReusableStringBuilder : ReusableResourceStore<StringBuilder, Int32>
        {
        private static readonly ReusableStringBuilder defaultInstance = new ReusableStringBuilder(512);
        private readonly Int32 maximumCacheCapacity;

        public ReusableStringBuilder(Int32 maximumCacheCapacity = 512)
            {
            Validate.IsWithinRange(maximumCacheCapacity, 1, Int32.MaxValue, "maximumCacheCapacity");
            this.maximumCacheCapacity = maximumCacheCapacity;
            }

        public static ReusableResourceHolder<StringBuilder> AcquireDefault(Int32 capacity)
            {
            return defaultInstance.Acquire(capacity);
            }

        protected override StringBuilder Allocate(Int32 constructorParameter)
            {
            return new StringBuilder(constructorParameter);
            }

        protected override Boolean Cleanup(StringBuilder value)
            {
            if (value.Capacity > maximumCacheCapacity)
                return false;
            value.Clear();
            return true;
            }

        protected override Boolean CanReuse(StringBuilder value, Int32 parameter)
            {
            return value.Capacity >= parameter;
            }
        }
    }