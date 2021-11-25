using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace BinaryStudio.PortableExecutable.TypeLibrary.MSFT
    {
    internal class MSFTTypeReference : ITypeLibraryTypeReference
        {
        public ITypeLibraryTypeDescriptor Type { get; }
        public IMPLTYPEFLAGS Flags { get; }

        public MSFTTypeReference(ITypeLibraryTypeDescriptor type, IMPLTYPEFLAGS flags)
            {
            Type = type;
            Flags = flags;
            }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            var r = new StringBuilder();
            var flags = Flags;
            var items = new List<String>();
            if (flags.Equals(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT))    { items.Add("default");    }
            if (flags.Equals(IMPLTYPEFLAGS.IMPLTYPEFLAG_FRESTRICTED)) { items.Add("restricted"); }
            if (flags.Equals(IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE))     { items.Add("source");     }
            if (items.Count > 0) {
                r.Append('[');
                r.Append(String.Join(",", items));
                r.Append("] ");
                }
            r.Append(Type);
            return r.ToString();
            }
        }
    }