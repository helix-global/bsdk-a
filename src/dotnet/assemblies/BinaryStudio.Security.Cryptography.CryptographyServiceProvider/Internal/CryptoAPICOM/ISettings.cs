using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [Guid("A24104F5-46D0-4C0F-926D-665565908E91")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual)]
    [ComImport]
    public interface ISettings
        {
        [DispId(1)]
        Boolean EnablePromptForCertificateUI
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(2)]
        CAPICOM_ACTIVE_DIRECTORY_SEARCH_LOCATION ActiveDirectorySearchLocation
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }
        }
    }
