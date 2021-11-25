using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("Certificates"), Guid("F701F8EC-31C7-48FB-B621-5DE417C3A607")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    public interface IStore3 : IStore2
        {
        [DispId(7)]
        String Name
            {
            [DispId(7)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(8)]
        CAPICOM_STORE_LOCATION Location
            {
            [DispId(8)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(9)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        Boolean Delete();

        [DispId(10)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Close();
        }
    }
