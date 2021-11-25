using System;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    public class CryptBlock<T>
        {
        public Int32 Index { get; }
        public T Block { get; }

        public CryptBlock(Int32 index, T block)
            {
            Index = index;
            Block = block;
            }
        }
    }