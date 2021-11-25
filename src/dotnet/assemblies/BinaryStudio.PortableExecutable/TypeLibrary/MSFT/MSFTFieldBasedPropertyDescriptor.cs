using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class MSFTFieldBasedPropertyDescriptor : TypeLibraryPropertyDescriptor
        {
        public MSFTFieldBasedPropertyDescriptor(ITypeLibraryTypeDescriptor type, ITypeLibraryFieldDescriptor fi)
            : base(type)
            {
            if (fi == null) { throw new ArgumentNullException(nameof(fi)); }
            CustomAttributes = fi.CustomAttributes;
            PropertyType = fi.FieldType;
            Name = fi.Name;
            HelpString = fi.HelpString;
            Id = fi.Id;
            CanWrite = !fi.Flags.HasFlag(TypeLibVarFlags.FReadOnly);
            }

        public override String Name { get; }
        public override String HelpString { get; }
        public override Int32 Id { get; }
        public override ITypeLibraryTypeDescriptor PropertyType { get; }
        public override ITypeLibraryMethodDescriptor GetMethod { get { return null; }}
        public override ITypeLibraryMethodDescriptor SetMethod { get { return null; }}
        public override Boolean CanRead  { get { return true; }}
        public override Boolean CanWrite { get; }
        public override IList<TypeLibraryCustomAttribute> CustomAttributes { get; }
        }
    }