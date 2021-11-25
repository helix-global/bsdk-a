using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("Certificate"), Guid("625B1F55-C720-41D6-9ECF-BA59F9B85F17")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual)]
    [ComImport]
    public interface ISigner2 : ISigner
        {
        [DispId(2)]
        IChain Chain
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(3)]
        CAPICOM_CERTIFICATE_INCLUDE_OPTION Options
            {
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Load([MarshalAs(UnmanagedType.BStr)] [In] String FileName, [MarshalAs(UnmanagedType.BStr)] [In] String Password = "");
        }
    }
