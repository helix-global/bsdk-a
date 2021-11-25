using System;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal class StreamOutputEventArgs : EventArgs
        {
        public Byte[] Block { get; }
        public Boolean IsFinal { get; }

        internal StreamOutputEventArgs(Byte[] block, Boolean final)
            {
            Block = block;
            IsFinal = final;
            }
        }
    }