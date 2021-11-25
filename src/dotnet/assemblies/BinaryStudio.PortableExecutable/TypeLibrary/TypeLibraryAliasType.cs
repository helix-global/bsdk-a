using System;

namespace BinaryStudio.PortableExecutable
    {
    internal class TypeLibraryAliasType : TypeLibraryTypeReference
        {
        public override String Name { get; }
        public override Boolean IsAlias { get { return true; }}

        public TypeLibraryAliasType(String name, ITypeLibraryTypeDescriptor type)
            :base(type)
            {
            if (String.IsNullOrWhiteSpace(name)) { throw new ArgumentOutOfRangeException(nameof(name)); }
            Name = name;
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return Name;
            }
        }
    }