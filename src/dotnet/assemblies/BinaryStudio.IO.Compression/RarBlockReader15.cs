using System;
using System.IO;
using System.Text;

namespace BinaryStudio.IO.Compression
    {
    internal class RarBlockReader15 : RarBlockReader
        {
        public RarBlockReader15(Encoding encoding, BinaryReader reader)
            : base(encoding, reader)
            {
            }

        public override Int32 MarkHeadSize { get { return 7; }}

        public override RarBaseBlock NextBlock()
            {
            throw new NotImplementedException();
            }
        }
    }