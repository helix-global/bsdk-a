using System;

namespace BinaryStudio.IO.Compression
    {
    internal class RarEndOfArcBlock : RarBaseBlock
        {
        public Boolean NextVolume { get;internal set; }
        }
    }