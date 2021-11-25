using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [Guid("BB3ECB9C-A83A-445C-BDB5-EFBEF691B731")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface ICertStore
        {
        [DispId(1610678272)]
        Int32 StoreHandle
            {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(1610678274)]
        CAPICOM_STORE_LOCATION StoreLocation
            {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [MethodImpl(MethodImplOptions.InternalCall)]
        void CloseHandle([In] Int32 hCertStore);
        }
    }
