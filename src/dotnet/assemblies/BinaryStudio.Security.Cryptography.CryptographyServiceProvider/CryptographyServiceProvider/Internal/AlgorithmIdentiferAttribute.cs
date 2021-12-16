using System;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    [AttributeUsage(AttributeTargets.Class)]
    internal class AlgorithmIdentiferAttribute : Attribute
        {
        public String AlgorithmIdentifer { get; }
        public AlgorithmIdentiferAttribute(String identifer)
            {
            AlgorithmIdentifer = identifer;
            }
        }
    }