using System;
using BinaryStudio.Security.Cryptography.Certificates;

namespace Options
    {
    internal class StoreLocationOption : OperationOption
        {
        public X509StoreLocation Value { get; }
        public StoreLocationOption(X509StoreLocation value) {
            Value = value;
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         */
        public override String ToString()
            {
            return $"storelocation:{Value}";
            }
        }
    }