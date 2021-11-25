using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("Certificates"), Guid("77F6F881-5D3A-4F2F-AEF0-E4A2F9AA689D")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual)]
    [ComImport]
    public interface IChain
        {
        [DispId(0)]
        ICertificates Certificates
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(1)]
        Int32 Status
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        Boolean Build([MarshalAs(UnmanagedType.Interface)] [In] ICertificate pICertificate);
        }
    }
