using System;
using System.IO;
using System.Text;

namespace BinaryStudio.IO.Compression
    {
    internal abstract class RarBlockReader
        {
        public Encoding Encoding { get; }
        public BinaryReader Reader { get; }
        protected RarBlockReader(Encoding encoding, BinaryReader reader)
            {
            Encoding = encoding;
            Reader = reader;
            }

        public abstract Int32 MarkHeadSize { get; }
        public abstract RarBaseBlock NextBlock();
        }
    }