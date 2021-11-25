using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("OID"), Guid("8973710C-8411-4951-9E65-D45FD524FFDF")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable|TypeLibTypeFlags.FDual)]
    [ComImport]
    public interface IPolicyInformation
        {
        [DispId(0)]
        IOID OID
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(1)]
        IQualifiers Qualifiers
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }
        }
    }
