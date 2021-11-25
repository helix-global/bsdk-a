using System;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    public class MemoryBlock
        {
        public IntPtr Block { get; }
        public Int64 Size { get; }
        public MemoryBlock(IntPtr block, Int64 size)
            {
            Block = block;
            Size = size;
            }
        }
    }