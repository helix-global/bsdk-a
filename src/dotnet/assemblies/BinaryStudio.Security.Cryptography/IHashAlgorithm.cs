using System;
using System.IO;
using System.Security.Cryptography;

namespace BinaryStudio.Security.Cryptography
    {
    public interface IHashAlgorithm : ICryptoTransform
        {
        Int32 HashSize { get; }
        Byte[] Hash { get; }
        /// <summary>Initializes an implementation of the <see cref="IHashAlgorithm"/> class.</summary>
        void Initialize();
        Byte[] ComputeHash(Stream inputstream);
        Byte[] Compute(Byte[] bytes);
        void CreateSignature(Stream signature, KeySpec keyspec);
        }
    }