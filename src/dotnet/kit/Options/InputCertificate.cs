using System;

namespace Options
    {
    internal class InputCertificate
        {
        public String Thumbprint { get;set; }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         */
        public override String ToString()
            {
            return Thumbprint;
            }
        }
    }