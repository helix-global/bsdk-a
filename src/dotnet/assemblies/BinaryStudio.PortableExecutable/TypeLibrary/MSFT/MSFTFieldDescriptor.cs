using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class MSFTFieldDescriptor : TypeLibraryFieldDescriptor
        {
        private const Int32 HelpStringOffset = 24;
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        private struct MSFT_PROPERTY_ENTRY
            {
            [FieldOffset( 0)] public  readonly Int16  RecordSize;        /* record size including some extra stuff */
            [FieldOffset( 4)] public  readonly Int32  DataType;          /* data type of the variable */
            [FieldOffset( 8)] public  readonly UInt16 Flags;             /* flags */
            [FieldOffset(12)] public  readonly UInt16 VarKind;           /* the kind of variable. */
            [FieldOffset(14)] private readonly Int16  vardescsize;       /* size of reconstituted VARDESC and related structs */
            [FieldOffset(16)] public  readonly Int32  ValueOffset;       /* value of the variable or the offset  */
            /* in the data structure */
            /* optional attribute fields, the number of them is variable */
            /* controlled by record length */
            [FieldOffset(20)] private readonly Int32  HelpContext;
            [FieldOffset(HelpStringOffset)] public readonly Int32   HelpString;
            [FieldOffset(32)] private readonly Int32  oCustData;        /* custom data for variable */
            [FieldOffset(36)] private readonly Int32  HelpStringContext;
            }

        public unsafe MSFTFieldDescriptor(MSFTMetadataTypeLibrary library, ITypeLibraryTypeDescriptor type, Byte* body, Int32 nameindex, Int32 index)
            :base(type)
            {
            var pi = (MSFT_PROPERTY_ENTRY*)body;
            var sz = pi->RecordSize;
            Name = library.N[nameindex];
            Flags = (TypeLibVarFlags)pi->Flags;
            Attributes = (TypeLibraryFieldAttributes)pi->VarKind;
            Id = index;
            if (sz >= HelpStringOffset + sizeof(Int32)) { HelpString = library.S[pi->HelpString]; }
            FieldType = library.TypeOf(pi->DataType);
            if (type.IsEnum) {
                IsLiteral = true;
                if ((pi->ValueOffset & 0x80000000) == 0x80000000) {
                    LiteralValue = (0x3FFFFFF & pi->ValueOffset);
                    }
                else
                    {
                    LiteralValue = library.Decode(library.C.Source, pi->ValueOffset);
                    }
                }
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return IsLiteral
                ? $"{Name} = {LiteralValue}"
                : base.ToString();
            }

        public override String Name { get; }
        public override String HelpString { get; }
        public override Int32 Id { get; }
        public override ITypeLibraryTypeDescriptor FieldType { get; }
        public override TypeLibVarFlags Flags { get; }
        public override TypeLibraryFieldAttributes Attributes { get; }
        public override Boolean IsLiteral { get; }
        public override Object LiteralValue { get; }
        }
    }