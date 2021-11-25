using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using CALLCONV = System.Runtime.InteropServices.ComTypes.CALLCONV;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class MSFTMethodDescriptor : TypeLibraryMethodDescriptor
        {
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        private struct MSFT_FUNCTION_ENTRY
            {
            [FieldOffset( 0)] public  readonly UInt32 RecordSize;
            [FieldOffset( 4)] public  readonly Int32  DataType;
            [FieldOffset( 8)] public  readonly UInt32 Flags;
            [FieldOffset(12)] private readonly UInt16 VirtualTable;
            [FieldOffset(14)] private readonly UInt16 FuncDescSize;
            [FieldOffset(16)] public  readonly UInt32 FKCCIC;
            [FieldOffset(20)] public  readonly UInt16 ParameterCount;
            [FieldOffset(22)] private readonly UInt16 OptionalParameterCount;
            [FieldOffset(24)] private readonly UInt32 HelpContext;
            [FieldOffset(28)] public  readonly Int32  HelpStringIndex;
            [FieldOffset(32)] private readonly UInt32 Entry;
            [FieldOffset(44)] private readonly UInt32 HelpStringContext;
            [FieldOffset(48)] public  readonly Int32  CustomDataOffset;
            }

        public unsafe MSFTMethodDescriptor(MSFTMetadataTypeLibrary library, ITypeLibraryTypeDescriptor typeinfo, Byte* body, Int32 nameindex, Int32 index)
            :base(typeinfo)
            {
            CustomAttributes = EmptyList<TypeLibraryCustomAttribute>.Value;
            var methodinfo = (MSFT_FUNCTION_ENTRY*)body;
            Name  = library.N[nameindex];
            Flags = (TypeLibFuncFlags)methodinfo->Flags;
            Attributes = (TypeLibraryMethodAttributes)((methodinfo->FKCCIC >> 3) & 0x0F);
            Id = index;
            var size   = (Int32)(methodinfo->RecordSize & 0xFFFF);
            var msb    = (methodinfo->FKCCIC & 0x8000) == 0x8000;
            var ein    = (methodinfo->FKCCIC & 0x2000) == 0x2000;
            var deflt  = (methodinfo->FKCCIC & 0x1000) == 0x1000;
            var custom = (methodinfo->FKCCIC & 0x0080) == 0x0080;
            var right  = size - sizeof(UInt32)*6 - sizeof(MSFTParameterDescriptor.MSFT_PARAMETER_INFO)*methodinfo->ParameterCount;
            if (right > 0) {
                if (msb) {
                    /* If the MSB = 1, then the default value is in the lower three bytes, otherwise, it's an offset
                        into the Custom Data Table where the default value can be found. */
                    right -= methodinfo->ParameterCount*sizeof(UInt32);
                    }
                if (right > 0) {
                    HelpString = library.S[methodinfo->HelpStringIndex];
                    right -= sizeof(UInt32);
                    }
                if ((right > 0) && (custom)) {
                    if (methodinfo->CustomDataOffset >= 0) {
                        CustomAttributes = library.C[methodinfo->CustomDataOffset];
                        }
                    }
                }
            var parameters = new ITypeLibraryParameterDescriptor[methodinfo->ParameterCount];
            if (methodinfo->ParameterCount > 0) {
                var pi = (MSFTParameterDescriptor.MSFT_PARAMETER_INFO*)((Byte*)methodinfo + size - sizeof(MSFTParameterDescriptor.MSFT_PARAMETER_INFO)*methodinfo->ParameterCount);
                for (var i = 0; i < methodinfo->ParameterCount; ++i) {
                    parameters[i] = new MSFTParameterDescriptor(library, this, pi, i);
                    pi++;
                    }
                }
            Parameters = new ReadOnlyCollection<ITypeLibraryParameterDescriptor>(parameters);
            ReturnType = library.TypeOf(methodinfo->DataType);
            }

        public override String Name { get; }
        public override String HelpString { get; }
        public override TypeLibraryMethodAttributes Attributes { get; }
        public override ITypeLibraryTypeDescriptor ReturnType { get; }
        public override Int32 Id { get; }
        public override TypeLibFuncFlags Flags { get; }
        public override CALLCONV CallingConvention { get; }
        public override IList<ITypeLibraryParameterDescriptor> Parameters { get; }
        public override IList<TypeLibraryCustomAttribute> CustomAttributes { get; }
        }
    }