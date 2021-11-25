using System;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class SLTGTypeReference : TypeLibraryTypeReference
        {
        public override ITypeLibraryTypeDescriptor UnderlyingType { get { return TypeReference; }}
        public ITypeLibraryTypeDescriptor TypeReference { get;set; }

        public SLTGTypeReference()
            : base(null)
            {
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return $"{Name}";
            }
        }
    }