using System;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable
    {
    public class Symbol : BaseSymbol
        {
        public String Name { get; }
        public Int16 SectionNumber { get; }
        public Int32 Value { get; }
        public IMAGE_SYM_CLASS StorageClass { get; }
        public Int32 NameIndex { get; }
        internal unsafe Symbol(IMAGE_SYMBOL* source, String name)
            {
            Name = name;
            StorageClass = source->StorageClass;
            SectionNumber = source->SectionNumber;
            Value = source->Value;
            NameIndex = source->Long;
            }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return Name;
            }
        }
    }