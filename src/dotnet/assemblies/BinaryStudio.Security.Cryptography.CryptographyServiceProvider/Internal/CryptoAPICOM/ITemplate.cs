using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [Guid("5F10FFCE-C922-476F-AA76-DF99D5BDFA2C")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable|TypeLibTypeFlags.FDual)]
    [ComImport]
    public interface ITemplate
        {
        [DispId(1)]
        Boolean IsPresent
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(2)]
        Boolean IsCritical
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(3)]
        String Name
            {
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(4)]
        IOID OID
            {
            [DispId(4)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(5)]
        Int32 MajorVersion
            {
            [DispId(5)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(6)]
        Int32 MinorVersion
            {
            [DispId(6)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }
        }
    }
