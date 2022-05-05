using System;

namespace Options
    {
    internal class ProviderTypeOption : OperationOption
        {
        public Int32 Type { get; }
        public ProviderTypeOption(Int32 type)
            {
            Type = type;
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         */
        public override String ToString()
            {
            return $"provider:{Type}";
            }
        }
    }