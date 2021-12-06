using System;
using System.Collections.Generic;

namespace Options
    {
    internal class InputCertificateOption : OperationOption
        {
        public IList<InputCertificate> Certificates { get; }
        public InputCertificateOption()
            {
            Certificates = new List<InputCertificate>();
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         */
        public override String ToString()
            {
            return $"certificate:{String.Join(",", Certificates)}";
            }
        }
    }